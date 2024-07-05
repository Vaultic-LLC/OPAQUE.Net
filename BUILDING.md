In order to make sure the package can run on both linux and windows, a .dll and .so needs to be generated.

### Windows
Assuming the package is being generated on a windows machine, run 
`cargo build`

This will generate a .dll file in target/debug

### Linux
Run `cross build --target aarch64-unknown-linux-gnu`

This will generate a .so file in target/aarch64-unknown-linux-gnu/debug

Take the .dll and .so file and replace the current opaque.dll and libopaque.so with them. For both, set their build action 'Copy to Output Directory' to 'Always'.