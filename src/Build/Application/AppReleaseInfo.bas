Attribute VB_Name = "AppReleaseInfo"
Option Explicit

'=========================================================================
' Module: AppReleaseInfo
' Layer: Application
' Responsibility: Central VMF Studio release metadata.
'=========================================================================

Private Const RELEASE_PRODUCT_NAME As String = "VMF Studio"
Private Const RELEASE_PRODUCT_VERSION As String = "1.1.0"
Private Const RELEASE_MANIFEST_SCHEMA_VERSION As String = "1.0"
Private Const RELEASE_TEMPLATE_SCHEMA_VERSION As String = "1.0"
Private Const RELEASE_MINIMUM_SUPPORTED_VERSION As String = "1.0"
Private Const RELEASE_BUILD_DATE As String = "2026-07-16"

Public Function AppProductName() As String
    AppProductName = RELEASE_PRODUCT_NAME
End Function

Public Function AppProductVersion() As String
    AppProductVersion = RELEASE_PRODUCT_VERSION
End Function

Public Function AppManifestSchemaVersion() As String
    AppManifestSchemaVersion = RELEASE_MANIFEST_SCHEMA_VERSION
End Function

Public Function AppTemplateSchemaVersion() As String
    AppTemplateSchemaVersion = RELEASE_TEMPLATE_SCHEMA_VERSION
End Function

Public Function AppMinimumSupportedVersion() As String
    AppMinimumSupportedVersion = RELEASE_MINIMUM_SUPPORTED_VERSION
End Function

Public Function AppBuildDate() As String
    AppBuildDate = RELEASE_BUILD_DATE
End Function
