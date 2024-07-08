In order to make sure the package can run on both linux and windows, a .dll and .so needs to be generated.

### Windows
Assuming the package is being generated on a windows machine, run 
`cargo build`

This will generate a .dll file in target/debug

### Linux
If you haven't used rust cross before, follow the getting started steps here https://github.com/cross-rs/cross/blob/main/docs/getting-started.md

Once everything is setup, run `cross build --target x86_64-unknown-linux-gnu`. If you get any errors, try running `cargo clean` and re trying.

This will generate a .so file in target/x86_64-unknown-linux-gnu/debug

Replace the current libopaque.so file with the new one.