namespace EmmaLykkeMadBlog.Services.Interfaces;

public interface IVisitorIdService
{
    // An anonymous value for counting unique visitors. It cannot be turned back into an address,
    // and it is different on every new day.
    string Create(string address, string userAgent);
}