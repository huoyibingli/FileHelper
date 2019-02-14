
namespace System
{
    public static class StringExtension
    {
        //假设需要format的字符串是10个，如果这10字符串累加起来的字符数不超过80，就能发挥StringBuilder的最佳性能；否则，StringBuider需要扩容，从而带来性能损失。
        //单个格式字符串扩展方法
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null || args == null)
            {
                throw new ArgumentNullException((format == null) ? "format" : "args");
            }
            else
            {
                var capacity = format.Length + args.Where(a => a != null).Select(p => p.ToString()).Sum(p => p.Length);
                Console.WriteLine(capacity);
                var stringBuilder = new StringBuilder(capacity);
                stringBuilder.AppendFormat(format, args);
                return stringBuilder.ToString();
            }
        }    

        //多个格式字符串拓展方法
        public static string FormatWith(this IEnumerable<string> formats, params object[] args)
        {
            if (formats == null || args == null)
            {
                throw new ArgumentNullException((formats == null) ? "formats" : "args");
            }
            else
            {
                var capacity = formats.Where(f => !string.IsNullOrEmpty(f)).Sum(f => f.Length) +
                    args.Where(a => a != null).Select(p => p.ToString()).Sum(p => p.Length);
                var stringBuilder = new StringBuilder(capacity);
                foreach (var f in formats)
                {
                    if (!string.IsNullOrEmpty(f))
                    {
                        stringBuilder.AppendFormat(f, args);
                    }
                }
                return stringBuilder.ToString();
            }
        } 


        //调用示例
        new string[] { "welcome to {0}!", " welcome to {1}!" }.FormatWith("www.cnblogs.com", "q.cnblogs.com");

        Post.Text = new string[]{
        "<span id=\"comment_body_{0}\" class=\"blog_comment_body\">{1}</span><br/>",
        " <a href=\"javascript:void(0);\" class=\"comment_vote\" onclick=\"voteComment({0},'Digg')\">支持({2})</a>",
        " <a href=\"javascript:void(0);\" class=\"comment_vote\" onclick=\"voteComment({0},'Bury')\">反对({3})</a>"
        }.FormatWith(comment.ID, comment.Body, comment.DiggCount, comment.BuryCount);   
    }
}