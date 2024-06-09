use libc::c_char;
use std::ffi::CStr;
use std::ffi::CString;

pub fn rust_string_to_csharp_string_handle(s: String) -> *mut c_char {
    let c_str = CString::new(s).unwrap();
    c_str.into_raw()
}

pub fn csharp_string_to_rust_string(s: *const c_char) -> String {
    let c_str: &CStr = unsafe {
        assert!(!s.is_null());
        CStr::from_ptr(s)
    };

    return c_str.to_str().unwrap().to_owned();
}

pub fn try_csharp_string_to_rust_string(s: Option<*mut c_char>) -> Option<String> {
    if s.is_some() {
        return Some(csharp_string_to_rust_string(s.unwrap()));
    }

    return None;
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
