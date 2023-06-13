
# BitMono Deobfuscator

This is a deobfuscator tool for BitMono (https://github.com/sunnamed434/BitMono)

## Features

- Decrypts obfuscated strings
- Unhooks the hooker
- Converts Calli to Call
- Deletes Anti Debugger Breakpoint thing

## Proof

Before
![dnSpy-x86_4oGMfukmHO](https://github.com/Yeetret/BitCleaner/assets/122463441/ef13172e-2a57-4c86-967d-435956f9badb)

After
![dnSpy-x86_aX4Kou9wEM](https://github.com/Yeetret/BitCleaner/assets/122463441/1857de63-6c6f-40ce-8db5-f92aef293c4d)



## Usage

1. Clone the repository
2. Build the project
3. Run the executable with the path to the obfuscated assembly as the argument
4. The deobfuscated assembly will be saved as `[original file name]-bitCleaned.[extension]`

## Dependencies

- dnlib (https://github.com/0xd4d/dnlib)
- .NET 6.0

## Contributing

Contributions are welcome! If you find a bug or have a feature request, please open an issue. If you would like to contribute code, please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [dnlib](https://github.com/0xd4d/dnlib) - A .NET library for reading and writing .NET assemblies.
