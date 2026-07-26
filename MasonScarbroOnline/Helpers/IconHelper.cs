namespace MasonScarbroOnline.Helpers
{
    public class IconHelper
    {
        private static readonly Dictionary<string, string> IconMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "C#", "devicon-csharp-plain colored" },
            { "ASP.NET Core", "devicon-dotnetcore-plain" },
            { ".NET", "devicon-dotnetcore-plain" },
            { "EF Core", "devicon-entityframeworkcore-plain colored" },
            { "SQL Server", "devicon-microsoftsqlserver-plain" },
            { "PostgreSQL", "devicon-postgresql-plain" },
            { "Tailwind", "devicon-tailwindcss-plain" },
            { "Docker", "devicon-docker-plain" },
            { "React", "devicon-react-original" },
            { "JavaScript", "devicon-javascript-plain" },
            { "TypeScript", "devicon-typescript-plain" },
            { "Python", "devicon-python-plain" },
            { "Go", "devicon-go-plain" },
            { "Rust", "devicon-rust-plain" },
            { "Git", "devicon-git-plain" },
            { "HTML", "devicon-html5-plain" },
            { "CSS", "devicon-css3-plain" },
            { "C++", "devicon-cplusplus-plain colored" },
            { "Java", "devicon-java-plain colored" },
            { "Zig", "devicon-zig-plain colored" },
            { "PHP", "devicon-php-plain colored" },
            { "Ruby", "devicon-ruby-plain colored" },
            { "Swift", "devicon-swift-plain colored" },
            { "Kotlin", "devicon-kotlin-plain colored" },
            { "Scala", "devicon-scala-plain colored" },
            { "Haskell", "devicon-haskell-plain colored" },
            { "Lua", "devicon-lua-plain colored" },
            { "Perl", "devicon-perl-plain colored" },
            { "R", "devicon-r-original colored" },
            { "Dart", "devicon-dart-plain colored" },
            { "Elixir", "devicon-elixir-plain colored" },
            { "Clojure", "devicon-clojure-plain colored" },
            { "Blazor", "devicon-blazor-plain colored" },
            { "OpenGl", "devicon-opengl-plain colored" }
        };

        public static string? GetIconClass(string techName) =>
            IconMap.TryGetValue(techName.Trim(), out var cls) ? cls : null;

        public static List<string> Split(string techStack) =>
            techStack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
}

