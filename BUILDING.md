In order to make sure the package can run on all supported platforms, native libraries need to be generated for each target.

### Windows (x64 and ARM64)
All Windows DLLs can be cross-compiled from any platform using `cargo-zigbuild` (see Linux section for install instructions).

Add the required Rust targets:
```
rustup target add x86_64-pc-windows-gnu aarch64-pc-windows-gnullvm
```

Build both release libraries:
```
cargo zigbuild --release --target x86_64-pc-windows-gnu
cargo zigbuild --release --target aarch64-pc-windows-gnullvm
```

This will generate:
- `target/x86_64-pc-windows-gnu/release/opaque.dll` → replace `.Net/OPAQUE.Net/opaque.dll`
- `target/aarch64-pc-windows-gnullvm/release/opaque.dll` → replace `.Net/OPAQUE.Net/opaque-arm64.dll`

### Linux (x64 and ARM64)
Install [Zig](https://ziglang.org/download/) and `cargo-zigbuild`:
```
cargo install cargo-zigbuild
```

Add the required Rust targets:
```
rustup target add x86_64-unknown-linux-gnu aarch64-unknown-linux-gnu
```

Build both release libraries:
```
cargo zigbuild --release --target x86_64-unknown-linux-gnu
cargo zigbuild --release --target aarch64-unknown-linux-gnu
```

This will generate:
- `target/x86_64-unknown-linux-gnu/release/libopaque.so` → replace `.Net/OPAQUE.Net/libopaque.so`
- `target/aarch64-unknown-linux-gnu/release/libopaque.so` → replace `.Net/OPAQUE.Net/libopaque-linux-arm64.so`

### macOS (ARM64 / Apple Silicon)
Ensure the `aarch64-apple-darwin` Rust target is installed:
```
rustup target add aarch64-apple-darwin
```

Then build the release library:
```
cargo build --release --target aarch64-apple-darwin
```

This will generate a .dylib file at `target/aarch64-apple-darwin/release/libopaque.dylib`.

Replace the current `.Net/OPAQUE.Net/libopaque.dylib` file with the new one.

### macOS (x64 / Intel)
Ensure the `x86_64-apple-darwin` Rust target is installed:
```
rustup target add x86_64-apple-darwin
```

Then build the release library:
```
cargo build --release --target x86_64-apple-darwin
```

This will generate a .dylib file at `target/x86_64-apple-darwin/release/libopaque.dylib`.

Replace the current `.Net/OPAQUE.Net/libopaque-x64.dylib` file with the new one.

