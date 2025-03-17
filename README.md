# DimonSmart.StringDiff


## Introduction


DimonSmart.StringDiff is a library for computing differences between strings, and creating difference models. Support for Markdown formatting.


[TRY online demo](https://dimonsmart.github.io/Demo/)

## Installation

You can install the DimonSmart.StringDiff package via NuGet Package Manager:

```sh
Install-Package DimonSmart.StringDiff
```

Or by using the .NET CLI:

```sh
dotnet add package DimonSmart.StringDiff
```

## Usage

Below is a guide on how to use the DimonSmart.StringDiff library to compute and visualize differences between strings.

### Basic Example


```csharp
using DimonSmart.StringDiff;

// Create an instance of StringDiff with default options
var stringDiff = new StringDiff();

// Compute the differences between two strings
var sourceText = "Hello, world!";
var targetText = "Hello, brave new world!";
var textDiff = stringDiff.ComputeDiff(sourceText, targetText);

// Display the differences
foreach (var edit in textDiff.Edits)
{
  Console.WriteLine($"StartPosition: {edit.StartPosition}, DeletedLength: {edit.DeletedLength}, InsertedText: {edit.InsertedText}");
}
```

### Custom Options

You can customize the behavior of the diff algorithm by providing `StringDiffOptions`.

```csharp
using DimonSmart.StringDiff;

// Create custom options
var options = new StringDiffOptions(10, DefaultTokenBoundaryDetector.Instance);

// Create an instance of StringDiff with custom options
var stringDiff = new StringDiff(options);

// Compute the differences between two strings
var sourceText = "123-456-7890";
var targetText = "123-456-0000";
var textDiff = stringDiff.ComputeDiff(sourceText, targetText);

// Display the differences
foreach (var edit in textDiff.Edits)
{
    Console.WriteLine($"StartPosition: {edit.StartPosition}, DeletedLength: {edit.DeletedLength}, InsertedText: {edit.InsertedText}");
}
```

## Visualizing Changes

To visualize changes, you can use the `StringReconstructor` class. This class helps to reconstruct the target string from the source string and a list of edits, applying formatting to the changes.

### Example

```csharp
using DimonSmart.StringDiff;

// Compute the differences
var stringDiff = new StringDiff();
var sourceText = "Hello, world!";
var targetText = "Hello, brave new world!";
var textDiff = stringDiff.ComputeDiff(sourceText, targetText);

// Reconstruct the target text with visual markers for changes
var reconstructor = StringReconstructor.Instance;
var visualizedText = reconstructor.Reconstruct(textDiff.Edits, sourceText);

Console.WriteLine(visualizedText);
```

### Markdown Support

You can also use the `MarkdownStringReconstructor` class to format the changes using Markdown syntax, which is useful for applications that support Markdown rendering.

```csharp
using DimonSmart.StringDiff;

// Compute the differences
var stringDiff = new StringDiff();
var sourceText = "Hello, world!";
var targetText = "Hello, brave new world!";
var textDiff = stringDiff.ComputeDiff(sourceText, targetText);

// Reconstruct the target text with Markdown formatting for changes
var markdownReconstructor = MarkdownStringReconstructor.Instance;
var visualizedMarkdownText = markdownReconstructor.Reconstruct(textDiff.Edits, sourceText);

Console.WriteLine(visualizedMarkdownText);
```

## API Reference

### StringDiff Class

The `StringDiff` class provides methods for computing differences between strings.

#### Constructors

- `StringDiff()`: Initializes a new instance of the `StringDiff` class with default options.
- `StringDiff(StringDiffOptions options)`: Initializes a new instance of the `StringDiff` class with custom options.

#### Methods

- `TextDiff ComputeDiff(string sourceText, string targetText)`: Computes the differences between the source and target texts and returns a `TextDiff` object.

### StringDiffOptions Record

The `StringDiffOptions` record allows customization of the diff algorithm.

#### Parameters

- `MinCommonLength (int)`: Forces the differ to report longer changes. For example, if set to 10 (phone number length), the differ will prefer not to report single-letter changes.
- `TokenBoundaryDetector (ITokenBoundaryDetector<string>?)`: The token boundary detector to use for detecting word or other token boundaries. If `null`, the default character-by-character tokenization will be used.

### TextDiff Class

The `TextDiff` class represents the result of a diff operation.

#### Properties

- `string SourceText`: The source text.
- `string TargetText`: The target text.
- `List<TextEdit> Edits`: A list of `TextEdit` objects representing the differences between the source and target texts.

### TextEdit Class

The `TextEdit` class represents a single text edit.

#### Properties

- `int Offset`: The offset position in the source text where the edit occurs.
- `int Length`: The length of the text to be replaced.
- `string Replacement`: The replacement text.

### ITokenBoundaryDetector Interface

The `ITokenBoundaryDetector<T>` interface defines a method for tokenizing input text into a sequence of tokens of type T.

#### Methods

- `HashSet<int> Detect(string s)`: Detects word boundaries in the specified string and returns a set of boundary indices.

### DefaultTokenBoundaryDetector Class

The `DefaultTokenBoundaryDetector` class provides a default implementation of `ITokenBoundaryDetector<string>` that splits text into word-like tokens.

#### Constructors

- `DefaultTokenBoundaryDetector()`: Initializes a new instance that splits text into words and non-word tokens.

### StringReconstructor Class

The `StringReconstructor` class helps to reconstruct the target string from the source string and a list of text edits. This class also provides methods for formatting the changes.

#### Methods

- `string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)`: Reconstructs the target string from the source string and a list of text edits.

#### Overridable Methods

- `protected virtual string FormatInsertedText(string text)`: Formats the inserted text. Default implementation returns the text as is.
- `protected virtual string FormatDeletedText(string text)`: Formats the deleted text. Default implementation returns an empty string.
- `protected virtual string FormatModifiedText(string oldText, string newText)`: Formats the modified text. Default implementation returns the new text.

### MarkdownStringReconstructor Class

The `MarkdownStringReconstructor` class extends the `StringReconstructor` class to provide Markdown formatting for changes.

#### Overridable Methods

- `protected override string FormatInsertedText(string text)`: Formats the inserted text using Markdown bold syntax (`**text**`).
- `protected override string FormatDeletedText(string text)`: Formats the deleted text using Markdown strikethrough syntax (`~~text~~`).
- `protected override string FormatModifiedText(string oldText, string newText)`: Formats the modified text using both Markdown strikethrough and bold syntax (`~~oldText~~**newText**`).

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue on GitHub.

## License

This project is licensed under the 0BSD License, the most permissive of all licenses. See the [LICENSE](https://opensource.org/licenses/0BSD) file for details.

## Acknowledgements

Special thanks to the contributors and the community for their valuable input and feedback.

---

DimonSmart.StringDiff is a powerful tool for computing and visualizing differences between text. It's ideal for use cases such as highlighting changes in edit history or comparing different versions of documents.