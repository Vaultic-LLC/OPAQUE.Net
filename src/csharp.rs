use libc::c_char;
use std::ffi::CStr;
use std::ffi::CString;

pub fn rust_string_to_csharp_string_handle(s: String) -> *mut c_char {
    CString::new(s)
        .map(CString::into_raw)
        .unwrap_or(std::ptr::null_mut())
}

pub fn csharp_string_to_rust_string(s: *const c_char) -> String {
    if s.is_null() {
        return String::new();
    }

    let c_str = unsafe { CStr::from_ptr(s) };
    c_str.to_str().unwrap_or_default().to_owned()
}

#[no_mangle]
pub extern "C" fn free_string(s: *mut c_char) {
    unsafe {
        if s.is_null() {
            return;
        }

        drop(CString::from_raw(s))
    };
}
