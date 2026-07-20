# VMF Studio Publisher PoC

This paragraph verifies **bold**, *italic*, ***bold italic***, and an
[inline link](https://example.com) in normal paragraph text.

This paragraph verifies __underscore bold__, _underscore italic_, and
___underscore bold italic__ syntax. Escaped markers remain plain characters:
\*not italic\*, \_not italic\_, and \[not a link\].

## **Styled heading** with _italic text_ and an [inline link](https://example.com/heading)

## Live verification checklist

- **Unordered level one** uses a disc.
  - _Unordered level two_ uses a circle.
    - ***Unordered level three*** uses a square.
      - [Unordered level four](https://example.com/unordered) remains nested.
1. **Ordered level one** uses a decimal marker.
  1. _Ordered level two_ uses an alpha marker.
    1. [***Ordered level three***](https://example.com/ordered) uses a Roman marker.
- A **mixed list** starts with an unordered item.
  1. Its _second level_ is ordered.
    - Its [**third level** returns to unordered](https://example.com/mixed).
      1. Its ___fourth level___ returns to ordered.

This **styled paragraph follows the list** and verifies that its position and
inline ranges are preserved after leading list tabs are removed.

Malformed constructs remain literal text: **unclosed bold, [](https://example.com),
and [invalid URL](relative/path).
