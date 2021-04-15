# Code Guidelines

Welcome to the Corona Check backend! If you'd like to contribute to the project then you're in the right place. This document details the rules and strategies which are applied during development of the backend.

During development we use an IDE (Visual Studio and Rider) along with ReShaper to help write good code. You can use Visual Studio Code but a full IDE is strongly recommended.

# Defensive programming

We apply a defensive programming strategy. This means that we assume that whatever can go wrong, will go wrong. All user input is hostile or just plain wrong and that all code is written by coked-up seat-of-the-underpants brogrammers. Finally, that we ourselves are fallible, writing code under extreme time constraints while having little sleep.

Some of the tools and practises we use:

* Code analysis turned up high.
* Well defined interfaces with bound and constrained input.
	* Strong typing (i.e. no string-typing)
	* Bound lengths, regex expressions.
	* Strong types favored over strings.
	* Strings constrained to the smallest possible alphabet.
	* Functions limited to the smallest possible domain.
* Review code with "what would Sauron do" mindset.
* Automated testing.
* Fuzz testing [note: at time of writing this is in the planning].

# Code style

Our code style is based on the standard stylings which ship with Visual Studio and the rules followed by the .Net Core and ASP.Net Core teams. This ensures that the code is as accessible to as many people as possible. 

## General rules

* Indent with 4 spaces.
* Normalize line-endings to LF when you commit.
* One class per file.
* Namespace matches the location.
* Use explicit accesibility modifiers (i.e. public, private, protected, internal)
* No public fields (use properties or expression body).
* Keep classes small and focussed.
* Keep code small and consice.
* Write integration/feature tests for public interfaces (i.e. web controllers).
	* Exercise as much of the stack as possible.
	* External processes (databases, caches, other webservices) can ba mocked.
* Write unit tests whenever possible.
	* Always include the happy flow
	* Strive to test the domain of the function
* Always check arguments.
* Use the null coalesense with throw expression pattern for null checks:
	_field = argument ?? throw new ArgumentNullException(nameof(argument))
* Use the ternary operator when possible, but try not to nest them.
* Green tick on all files (no errors/warnings flagged by ReSharper's static analysis).

## Braces

We use Allman style braces:

	namespace MyNamespace
	{
		public class Foo
		{
			public void Foo()
			{
			}
		}
	}

With a space between the statement and ther condition:

	if (true)
	{
		// ...
	}
	
	while (true)
	{
	}

Both one-line and two-line forms are allowed for small statements (use single-line if it fits)

	if (true) return;
	
	if (true && true && true)
		return;


## Ternary operator

Keep them short, if they're long then split them using the multi-line style below.

Single line:

	return true ? 1 : 0;
	
	
Multi-line:

	return true
		? 1
		: 0;

## Casing

Names of classes, structs, methods (and lambdas and other similar constructs), properties and constants are in *Pascal Case*

	class MyClass
	struct MyStruct
	void MyMethod()
	string MyProperty { get; set; }
	const int MyConst = 42;

Names of method arguments and local variables are in *Camel Case*:

	void *MyMethod*(string myArgument)
	{
		var *myVariable* = 100;
	}

Private fields are in *Camal Case* and prefixed with an underscore

	public class MyClass
	{
		private readonly _myField = 100;
	}

## Exception handling

In the general case we follow Microsoft's best practises, because they are solid engineering practise.

https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions

Our strategy is to only handle recoverable exceptions close to the point they occur, and recover them. Otherwise exceptions will bubble up to the general exception handler of the process. That handler must log the exception. There are of course a small number of occasions where you may need to catch an exception in order to provide logging (i.e where you would otherwise miss state) or where you're using APIs which explicitly require it. In those cases the exceptions should be caught, your logging made, and then the exception **correctly** re-thrown.

So:

* Avoid exceptions where possible.
* Never throw exceptions which should only be thrown by the framework, such as: `IndexOutOfRangeException`, `NullReferenceException` or `Exception`.
* Handle exceptions in the execution context's root handler.
* NEVER include sensitive information in the exception message (or the logs)
* COMMENT any exceptions to the rule.
* Use custom exceptions sparingly.


# PR Process

Before a PR is submitted:

* All tests must pass
* No compiler warnings
* Security scan analyzer must return a clean
	* https://security-code-scan.github.io/
	* security-scan nl-covid19-coronacheck-app-backend.sln
	
We have several scripts which you can use to do this, just run them without arguments.

Build all of the publishable projects:

	build.bat 
	
Build then publish all of the publishable projects:

	publish.bat

Run all tests and the extra security scans:

	test-and-scan.bat
