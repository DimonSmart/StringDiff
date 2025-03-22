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

You can customize the behavior of the diff algorithm by providing `StringDiffOptions`:

```csharp
using DimonSmart.StringDiff;

// Create custom options with a tokenizer and entity detectors
var options = new StringDiffOptions(
    SimpleTokenizer.Instance,
    new IEntityDetector[] 
    { 
        new EmailEntityDetector(),
        new PhoneEntityDetector()
    });

// Create an instance of StringDiff with custom options
var stringDiff = new StringDiff(options);

// Compute the differences between two strings
var sourceText = "Contact me at old@email.com or 123-456-7890";
var targetText = "Contact me at new@email.com or 999-888-7777";
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

- `Tokenizer (ITokenizer?)`: The tokenizer to use for splitting text into tokens. If `null`, the default SimpleTokenizer will be used.
- `EntityDetectors (IEnumerable<IEntityDetector>?)`: Optional collection of entity detectors for recognizing special tokens like emails or phone numbers.

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

## Tokenization Architecture

The library uses a two-level tokenization system:

1. Base Tokenization (ITokenizer)
   - Handles basic text tokenization (words, spaces, punctuation)
   - Implemented by SimpleTokenizer (default)
   - Can be replaced with custom implementations

2. Entity Detection (IEntityDetector)
   - Detects special entities that should be treated as atomic units
   - Multiple detectors can be used together
   - Built-in detectors:
     - EmailEntityDetector (recognizes email addresses)
     - PhoneEntityDetector (recognizes phone numbers)

### SimpleTokenizer

The default tokenizer that splits text into words and non-word tokens using `char.IsLetterOrDigit`.

```csharp
var tokenizer = SimpleTokenizer.Instance;
```

### Entity Detectors

Specialized detectors for recognizing entities that should be kept as single tokens:

```csharp
// Email detector
var emailDetector = new EmailEntityDetector();
// Recognizes: "user@example.com" as a single token

// Phone detector
var phoneDetector = new PhoneEntityDetector();
// Recognizes: "+1-555-567-8900" as a single token
```

### TokenProcessor

Combines a base tokenizer with multiple entity detectors:

```csharp
var processor = new TokenProcessor(
    SimpleTokenizer.Instance,
    new IEntityDetector[] 
    { 
        new EmailEntityDetector(),
        new PhoneEntityDetector()
    });

// Example: "Contact user@example.com" 
// Tokens: ["Contact", " ", "user@example.com"]
```

### Custom Implementations

You can implement custom tokenizers and entity detectors:

```csharp
public class MyTokenizer : ITokenizer
{
    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        // Your tokenization logic here
    }
}

public class MyEntityDetector : IEntityDetector
{
    public void DetectEntities(ReadOnlySpan<char> text, Span<Range> entityRanges, out int entityCount)
    {
        // Your entity detection logic here
    }
}
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue on GitHub.

## License

This project is licensed under the 0BSD License, the most permissive of all licenses. See the [LICENSE](https://opensource.org/licenses/0BSD) file for details.

## Acknowledgements

Special thanks to the contributors and the community for their valuable input and feedback.

---

DimonSmart.StringDiff is a powerful tool for computing and visualizing differences between text. It's ideal for use cases such as highlighting changes in edit history or comparing different versions of documents.