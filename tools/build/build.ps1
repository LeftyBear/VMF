# Official VMF Build.xlam entry point. Substantive work is owned by Build.Host.
[CmdletBinding()]
param([string]$OutputPath,[string]$BuildVersion,[string]$ReleaseType="Release")
$ErrorActionPreference="Stop"
$scriptDir=Split-Path -Parent $MyInvocation.MyCommand.Path
function Resolve-VmfRepositoryRoot([string]$Start){$p=[IO.Path]::GetFullPath($Start);while($true){if(Test-Path -LiteralPath (Join-Path $p ".vmf-root") -PathType Leaf){return $p};$parent=Split-Path -Parent $p;if([string]::IsNullOrWhiteSpace($parent)-or$parent-eq$p){throw "VMF repository root could not be resolved."};$p=$parent}}
function Quote-NativeArgument([string]$Value){if($Value-notmatch'[\s\"]'){return $Value};return '"'+([regex]::Replace($Value,'(\\*)"','$1$1\"')-replace'(\\+)$','$1$1')+'"'}
function Get-ArtifactIdentity([string]$Path){if(-not(Test-Path -LiteralPath $Path -PathType Leaf)){return $null};$i=Get-Item -LiteralPath $Path;[pscustomobject]@{Path=$i.FullName;LastWriteUtc=$i.LastWriteTimeUtc;Length=$i.Length;Sha256=(Get-FileHash -LiteralPath $i.FullName -Algorithm SHA256).Hash}}
function Read-Lifecycle([string]$Path,[Guid]$Id,[DateTimeOffset]$InvocationStart,[DateTimeOffset]$InvocationDeadline){
 if(-not(Test-Path -LiteralPath $Path -PathType Leaf)){throw "Lifecycle evidence is missing."}
 $lines=@(Get-Content -LiteralPath $Path -Encoding UTF8);if($lines.Count-eq0){throw "Lifecycle evidence is empty."};$entries=@();$sequence=0;$priorState=$null;$priorTimestamp=$null;$now=[DateTimeOffset]::UtcNow
 $normalTransitions=@{
  InvocationStarted=@("ExcelActivationAboutToBegin","InvocationFailed");ExcelActivationAboutToBegin=@("ExcelOwnershipEstablished","InvocationFailed");ExcelOwnershipEstablished=@("ExcelAttachmentValidated","ExcelCleanupStarted","InvocationFailed");ExcelAttachmentValidated=@("ExcelOperationStarted","ExcelCleanupStarted","InvocationFailed");ExcelOperationStarted=@("ExcelCleanupStarted","InvocationFailed");ExcelCleanupStarted=@("ExcelCleanupCompleted","InvocationFailed");ExcelCleanupCompleted=@("ExcelResidualChecked","InvocationFailed");ExcelResidualChecked=@("InvocationSucceeded","InvocationFailed")
 }
 $timeoutEligible=@("InvocationStarted","ExcelActivationAboutToBegin","ExcelOwnershipEstablished","ExcelAttachmentValidated","ExcelOperationStarted")
 foreach($line in $lines){
  try{$e=$line|ConvertFrom-Json -ErrorAction Stop;$entryId=[Guid]$e.invocationId;$entrySequence=[long]$e.sequence;$entryTimestamp=[DateTimeOffset]::Parse([string]$e.recordedUtc,[Globalization.CultureInfo]::InvariantCulture,[Globalization.DateTimeStyles]::AssumeUniversal);$state=[string]$e.state}catch{throw "Lifecycle evidence is malformed."}
  if($e.schemaVersion-ne2-or$entryId-ne$Id-or$entrySequence-ne(++$sequence)-or[string]::IsNullOrWhiteSpace($state)){throw "Lifecycle evidence is unsupported, non-monotonic, or mismatched."}
  $entryTimestamp=$entryTimestamp.ToUniversalTime();if($entryTimestamp-lt$InvocationStart-or$entryTimestamp-gt$now-or($null-ne$priorTimestamp-and$entryTimestamp-lt$priorTimestamp)){throw "Lifecycle timestamp evidence is stale, future, or non-monotonic."}
  if($null-eq$priorState){if($state-ne"InvocationStarted"){throw "Lifecycle initial state is invalid."}}
  elseif($state-eq"LauncherDeadlineExpired"){if($priorState-notin$timeoutEligible-or$entryTimestamp-lt$InvocationDeadline){throw "Lifecycle deadline transition is invalid."}}
  elseif($priorState-eq"LauncherDeadlineExpired"){if($state-ne"CleanupRequested"){throw "Lifecycle cleanup-request transition is invalid."}}
  elseif($priorState-eq"CleanupRequested"){if($state-notin@("ExcelCleanupStarted","HardTerminationCompleted","InvocationFailed")){throw "Lifecycle post-request transition is invalid."}}
  elseif($priorState-eq"HardTerminationCompleted"){if($state-notin@("ExcelResidualChecked","InvocationFailed")){throw "Lifecycle post-termination transition is invalid."}}
  elseif(-not$normalTransitions.ContainsKey($priorState)-or$state-notin$normalTransitions[$priorState]){throw "Lifecycle transition is invalid."}
  $entries+=$e;$priorState=$state;$priorTimestamp=$entryTimestamp
 }
 return,$entries
}
function Test-OwnershipMatch($Actual,$Expected,[Guid]$Id){
 try{return $null-ne$Actual-and[Guid]$Actual.invocationId-eq$Id-and[int]$Actual.processId-eq[int]$Expected.processId-and([DateTimeOffset]$Actual.processStartedUtc).UtcDateTime-eq([DateTimeOffset]$Expected.processStartedUtc).UtcDateTime-and[IO.Path]::GetFullPath([string]$Actual.executablePath)-eq[IO.Path]::GetFullPath([string]$Expected.executablePath)-and[IO.Path]::GetFullPath([string]$Actual.sentinelPath)-eq[IO.Path]::GetFullPath([string]$Expected.sentinelPath)}catch{return $false}
}
function Get-ValidatedExcelProcess($Ownership,[Guid]$Id,[string]$LifecyclePath,[string]$ExpectedSentinelPath,[DateTimeOffset]$InvocationStart,[DateTimeOffset]$InvocationDeadline){
 $process=$null
 try{
  if($null-eq$Ownership-or[Guid]$Ownership.invocationId-ne$Id){return $null};$sentinel=[IO.Path]::GetFullPath([string]$Ownership.sentinelPath);if($sentinel-ne[IO.Path]::GetFullPath($ExpectedSentinelPath)-or-not(Test-Path -LiteralPath $sentinel -PathType Leaf)){return $null}
  $entries=Read-Lifecycle $LifecyclePath $Id $InvocationStart $InvocationDeadline;if($entries[-1].state-ne"CleanupRequested"){return $null}
  $established=@($entries|Where-Object{$_.state-eq"ExcelOwnershipEstablished"});$attached=@($entries|Where-Object{$_.state-eq"ExcelAttachmentValidated"});if($established.Count-ne1-or$attached.Count-ne1-or-not(Test-OwnershipMatch $established[0].ownership $Ownership $Id)-or-not(Test-OwnershipMatch $attached[0].ownership $Ownership $Id)){return $null}
  $process=[Diagnostics.Process]::GetProcessById([int]$Ownership.processId);$process.Refresh();if($process.HasExited){return $null};$actualStart=[DateTimeOffset]$process.StartTime.ToUniversalTime();$expectedStart=[DateTimeOffset]$Ownership.processStartedUtc;$actualExe=[IO.Path]::GetFullPath($process.MainModule.FileName);$expectedExe=[IO.Path]::GetFullPath([string]$Ownership.executablePath)
  if($process.Id-ne[int]$Ownership.processId-or$actualStart.UtcDateTime-ne$expectedStart.UtcDateTime-or$actualExe-ne$expectedExe-or[IO.Path]::GetFileName($actualExe)-ine"EXCEL.EXE"){return $null};$validated=$process;$process=$null;return $validated
 }catch{return $null}finally{if($null-ne$process){$process.Dispose()}}
}
try{
 $root=Resolve-VmfRepositoryRoot $scriptDir;$source=Join-Path $root "src\Build";$host=Join-Path $root "src\Build.Host\bin\Release\net8.0-windows\Vmf.Build.Host.dll"
 if(-not(Test-Path -LiteralPath $host -PathType Leaf)){throw "Approved Build.Host not found: $host"};if(-not(Test-Path -LiteralPath $source -PathType Container)){throw "Build source directory not found: $source"};if([string]::IsNullOrWhiteSpace($ReleaseType)){throw "ReleaseType is required."}
 $release=Get-Content -LiteralPath (Join-Path $source "Application\AppReleaseInfo.bas") -Raw -Encoding Default;$m=[regex]::Match($release,'Private Const RELEASE_PRODUCT_VERSION As String = "([^"]+)"');if(-not$m.Success){throw "Release product version was not found."};$version=$m.Groups[1].Value;if([string]::IsNullOrWhiteSpace($BuildVersion)){$BuildVersion=$version}
 if([string]::IsNullOrWhiteSpace($OutputPath)){$folder=$version-replace'^(\d+\.\d+).*$','v$1';$OutputPath=Join-Path $root "dist\release\Build\$folder\Build.xlam"};$output=[IO.Path]::GetFullPath($OutputPath);if(Test-Path -LiteralPath $output -PathType Container){throw "OutputPath is a directory."}
 $id=[Guid]::NewGuid();$start=[DateTimeOffset]::UtcNow;$deadline=$start.AddSeconds(120);$inv=Join-Path ([IO.Path]::GetTempPath()) ("vmf-build-launcher\"+$id.ToString("N"));New-Item -ItemType Directory -Path $inv -Force|Out-Null
 $requestPath=Join-Path $inv "request.json";$resultPath=Join-Path $inv "result.json";$lifecyclePath=Join-Path $inv "lifecycle.jsonl";$logPath=Join-Path $inv "build.jsonl";$stdoutPath=Join-Path $inv "stdout.log";$stderrPath=Join-Path $inv "stderr.log";$previous=Get-ArtifactIdentity $output
 [ordered]@{schemaVersion=2;invocationId=$id;startedUtc=$start;deadlineUtc=$deadline;repositoryRoot=$root;sourceRoot=$source;outputPath=$output;buildVersion=$BuildVersion;releaseType=$ReleaseType;resultPath=$resultPath;lifecyclePath=$lifecyclePath;logPath=$logPath}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath $requestPath -Encoding UTF8
 $si=[Diagnostics.ProcessStartInfo]::new("dotnet",(Quote-NativeArgument $host)+" --request "+(Quote-NativeArgument $requestPath));$si.UseShellExecute=$false;$si.CreateNoWindow=$true;$si.RedirectStandardOutput=$true;$si.RedirectStandardError=$true;$p=[Diagnostics.Process]::new();$p.StartInfo=$si;if(-not$p.Start()){throw "Build.Host could not be started."};$outTask=$p.StandardOutput.ReadToEndAsync();$errTask=$p.StandardError.ReadToEndAsync();$milliseconds=[Math]::Max(1,[int]($deadline-[DateTimeOffset]::UtcNow).TotalMilliseconds)
 if(-not$p.WaitForExit($milliseconds)){
   $entries=Read-Lifecycle $lifecyclePath $id $start $deadline;$ownership=($entries|Where-Object{$null-ne$_.ownership}|Select-Object -Last 1).ownership
   @{schemaVersion=2;invocationId=$id;requestedUtc=[DateTimeOffset]::UtcNow}|ConvertTo-Json|Set-Content -LiteralPath (Join-Path $inv "cleanup.request.json") -Encoding UTF8
   Add-Content -LiteralPath $lifecyclePath -Encoding UTF8 -Value (([ordered]@{schemaVersion=2;invocationId=$id;sequence=$entries.Count+1;recordedUtc=[DateTimeOffset]::UtcNow;state="LauncherDeadlineExpired";ownership=$ownership}|ConvertTo-Json -Compress -Depth 8))
   Add-Content -LiteralPath $lifecyclePath -Encoding UTF8 -Value (([ordered]@{schemaVersion=2;invocationId=$id;sequence=$entries.Count+2;recordedUtc=[DateTimeOffset]::UtcNow;state="CleanupRequested";ownership=$ownership}|ConvertTo-Json -Compress -Depth 8))
   if(-not$p.WaitForExit(30000)){$expectedSentinel=Join-Path ([IO.Path]::GetTempPath()) ("vmf-build\"+$id.ToString("N")+"\sentinel-"+$id.ToString("N")+".xlsx");$excel=Get-ValidatedExcelProcess $ownership $id $lifecyclePath $expectedSentinel $start $deadline;if($null-ne$excel){try{$excel.Kill();$excel.WaitForExit();Add-Content -LiteralPath $lifecyclePath -Encoding UTF8 -Value (([ordered]@{schemaVersion=2;invocationId=$id;sequence=$entries.Count+3;recordedUtc=[DateTimeOffset]::UtcNow;state="HardTerminationCompleted";ownership=$ownership}|ConvertTo-Json -Compress -Depth 8));try{$p.Kill()}catch{};throw "Build.Host deadline expired; exact owned Excel was terminated after cleanup grace."}finally{$excel.Dispose()}}throw "SAFE-STOP: deadline expired and exact Excel ownership could not be revalidated; no Excel process was terminated. Human inspection required."};throw "Build.Host deadline expired; cooperative cleanup completed; artifact publication is prohibited."
 }
 $stdout=$outTask.Result;$stderr=$errTask.Result;Set-Content -LiteralPath $stdoutPath -Value $stdout -Encoding UTF8;Set-Content -LiteralPath $stderrPath -Value $stderr -Encoding UTF8;$exitCode=$p.ExitCode
 if(-not(Test-Path -LiteralPath $resultPath -PathType Leaf)){throw "Atomic structured result is missing."};$result=Get-Content -LiteralPath $resultPath -Raw -Encoding UTF8|ConvertFrom-Json
 if($result.schemaVersion-ne2-or[Guid]$result.invocationId-ne$id-or-not$result.complete){throw "Result evidence is malformed, incomplete, or uncorrelated."};if(($exitCode-eq0)-ne[bool]$result.success){throw "Host exit/result disagreement."};if($exitCode-ne0){throw "Build.Host failed: $($result.failureCategory): $($result.failureDetail)"};if(-not$result.releaseMetadataUpdated-or-not$result.cleanupSucceeded-or$result.residualOwnedExcelProcess-ne"Clear"){throw "Host lifecycle or residual evidence failed."}
 $artifact=Get-ArtifactIdentity $output;if($null-eq$artifact-or$artifact.Length-le0){throw "Output artifact is missing."};if([IO.Path]::GetFullPath([string]$result.artifact.path)-ne$output-or$artifact.Length-ne[long]$result.artifact.length-or$artifact.Sha256-ne[string]$result.artifact.sha256-or$artifact.LastWriteUtc-ne[datetime]$result.artifact.lastWriteUtc){throw "Independent final artifact identity verification failed."};if($artifact.LastWriteUtc-lt$start.UtcDateTime){throw "Output artifact is stale."};if($previous-and$previous.Sha256-eq$artifact.Sha256-and$previous.LastWriteUtc-eq$artifact.LastWriteUtc){throw "Output is ambiguous with the pre-existing artifact."}
 Write-Host "Invocation: $id";Write-Host "Artifact: $($artifact.Path)";Write-Host "Length: $($artifact.Length)";Write-Host "SHA-256: $($artifact.Sha256)";Write-Host "Source inventory SHA-256: $($result.sourceInventorySha256)";Write-Host "Evidence: $resultPath";exit 0
}catch{Write-Error $_;exit 1}
