namespace Nancy.MimeType
{
  /// <summary>
  /// A static class wrapper around the IMimeTypeRegistry functionality.
  /// </summary>
  public static class MimeTypeUtils
  {
    private static readonly IMimeTypeRegistry reg = new DefaultMimeTypeRegistry();
    public static string GetMimeTypeFor(string extension) {
      return reg.GetMimeTypeFor(extension);
    }
  }
}
