namespace EmmaLykkeMadBlog.DTOs;

// What a page needs to know about a call to the API. Pages switch on this and never catch exceptions.
public enum ApiStatus
{
    Ok,
    NotFound,
    Unavailable
}