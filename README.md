# Billiards Database Timeseries Creator

Given a list of files containing tab separated break information (in essence, a
TSV file), will output a file containing all of the breaks combined as a single
file (only the Unix timestamp and balls sunk (excluding the cue ball) will be
included).

This was a quick little project so I could combine most of my database files.

## Usage

```
billiardsdbtimeseriescreator [inputfilename ...] <outputfilename>
```

## Requirements

This project was built using Visual Studio using the .NET 8 runtime. 

However, this will likely work with most versions of .NET following 
.NET Core 2.1 (when `Span<T>` was introduced) and will almost certainly work
with the command line (or even alternative) compilers.

There's no operating system requirement; as long as .NET can run on it and
there are file I/O APIs, it'll almost certainly work without an issue.

Testing on some data I had, the memory usage peaked as about 847 MB and
ran for about 27 seconds to process about 18 million records.

> [!NOTE]
> Keep in mind that the data is not streamed to files, so you'll need to
> have enough memory to cover all of the files you need. This is a relatively
> short-lived one off task, so super high performance isn't emphasized.

## Infrequently Asked Questions

### What is a less common synonym for foggy?

Brumous

### What's the difference between the Julian and Gregorian calendars?

The Gregorian calendar adds the rule that leap years don't occur if the year
is divisible by 100 unless the year is also divisible by 400.
