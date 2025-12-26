using System.Text;
using System.Text.Json;

namespace OrkinosaiCMS.Web.Middleware;

/// <summary>
/// Middleware to display detailed exception information during development
/// Inspired by error harness patterns from mosaic/mosaic-saas-new repositories
/// </summary>
public class DevelopmentExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DevelopmentExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public DevelopmentExceptionMiddleware(
        RequestDelegate next,
        ILogger<DevelopmentExceptionMiddleware> logger,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var showDetailedErrors = _configuration.GetValue<bool>("ErrorHandling:ShowDetailedErrors", false);
        var includeStackTrace = _configuration.GetValue<bool>("ErrorHandling:IncludeStackTrace", false);

        // Log the exception with full details
        _logger.LogError(exception, 
            "Unhandled exception occurred. Path: {Path}, Method: {Method}, QueryString: {QueryString}",
            context.Request.Path,
            context.Request.Method,
            context.Request.QueryString);

        // If not development or detailed errors are disabled, use default error handling
        if (!_environment.IsDevelopment() && !showDetailedErrors)
        {
            context.Response.Redirect("/Error");
            return;
        }

        // Development mode or detailed errors enabled - show comprehensive error information
        context.Response.ContentType = "text/html";
        context.Response.StatusCode = 500;

        var errorHtml = GenerateDetailedErrorHtml(exception, context, includeStackTrace);
        await context.Response.WriteAsync(errorHtml);
    }

    private string GenerateDetailedErrorHtml(Exception exception, HttpContext context, bool includeStackTrace)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset=\"UTF-8\">");
        sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("    <title>Development Exception - OrkinosaiCMS</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
        sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #1a1a1a; color: #e0e0e0; padding: 20px; }");
        sb.AppendLine("        .container { max-width: 1200px; margin: 0 auto; }");
        sb.AppendLine("        .header { background: linear-gradient(135deg, #c62828, #d32f2f); color: white; padding: 30px; border-radius: 8px 8px 0 0; box-shadow: 0 4px 6px rgba(0,0,0,0.3); }");
        sb.AppendLine("        .header h1 { font-size: 28px; margin-bottom: 10px; display: flex; align-items: center; }");
        sb.AppendLine("        .header h1::before { content: '⚠️'; margin-right: 10px; font-size: 32px; }");
        sb.AppendLine("        .header p { font-size: 14px; opacity: 0.95; }");
        sb.AppendLine("        .warning-banner { background: #ffa726; color: #1a1a1a; padding: 15px; border-left: 5px solid #f57c00; margin: 20px 0; border-radius: 4px; }");
        sb.AppendLine("        .warning-banner strong { display: block; margin-bottom: 5px; font-size: 16px; }");
        sb.AppendLine("        .section { background: #2d2d2d; border: 1px solid #404040; border-radius: 8px; margin: 20px 0; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.2); }");
        sb.AppendLine("        .section-header { background: #333; color: #fff; padding: 15px 20px; font-weight: bold; font-size: 16px; border-bottom: 2px solid #0078D4; }");
        sb.AppendLine("        .section-content { padding: 20px; }");
        sb.AppendLine("        .exception-type { color: #ef5350; font-family: 'Courier New', monospace; font-size: 18px; font-weight: bold; margin-bottom: 10px; }");
        sb.AppendLine("        .exception-message { background: #1a1a1a; border-left: 4px solid #ef5350; padding: 15px; margin: 10px 0; font-family: 'Courier New', monospace; color: #ffcdd2; border-radius: 4px; }");
        sb.AppendLine("        .stack-trace { background: #1a1a1a; border: 1px solid #404040; padding: 15px; font-family: 'Courier New', monospace; font-size: 13px; overflow-x: auto; border-radius: 4px; color: #90caf9; white-space: pre-wrap; word-wrap: break-word; }");
        sb.AppendLine("        .stack-trace .file-path { color: #a5d6a7; }");
        sb.AppendLine("        .stack-trace .line-number { color: #ffcc80; }");
        sb.AppendLine("        .stack-trace .method { color: #ce93d8; }");
        sb.AppendLine("        .info-grid { display: grid; grid-template-columns: 200px 1fr; gap: 10px; }");
        sb.AppendLine("        .info-label { font-weight: bold; color: #0078D4; }");
        sb.AppendLine("        .info-value { color: #e0e0e0; font-family: 'Courier New', monospace; word-break: break-all; }");
        sb.AppendLine("        .inner-exception { margin-top: 15px; padding: 15px; background: #1a1a1a; border-left: 4px solid #ff9800; border-radius: 4px; }");
        sb.AppendLine("        .inner-exception-title { color: #ff9800; font-weight: bold; margin-bottom: 10px; font-size: 16px; }");
        sb.AppendLine("        .data-item { margin: 5px 0; padding: 8px; background: #1a1a1a; border-radius: 4px; }");
        sb.AppendLine("        .footer { text-align: center; padding: 20px; color: #888; font-size: 14px; }");
        sb.AppendLine("        .toggle-button { background: #0078D4; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; margin: 10px 0; font-size: 14px; }");
        sb.AppendLine("        .toggle-button:hover { background: #005a9e; }");
        sb.AppendLine("        .collapsible-content { display: none; margin-top: 10px; }");
        sb.AppendLine("        .collapsible-content.active { display: block; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("    <script>");
        sb.AppendLine("        function toggleSection(id) {");
        sb.AppendLine("            var content = document.getElementById(id);");
        sb.AppendLine("            content.classList.toggle('active');");
        sb.AppendLine("        }");
        sb.AppendLine("    </script>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class=\"container\">");
        sb.AppendLine("        <div class=\"header\">");
        sb.AppendLine("            <h1>Development Exception Occurred</h1>");
        sb.AppendLine("            <p>OrkinosaiCMS - Detailed Error Information for Debugging</p>");
        sb.AppendLine("        </div>");
        
        sb.AppendLine("        <div class=\"warning-banner\">");
        sb.AppendLine("            <strong>⚠️ DEVELOPMENT MODE ACTIVE</strong>");
        sb.AppendLine("            This detailed error page is only shown during development. In production, users will see a friendly error page.");
        sb.AppendLine("        </div>");

        // Exception Details Section
        sb.AppendLine("        <div class=\"section\">");
        sb.AppendLine("            <div class=\"section-header\">🔴 Exception Details</div>");
        sb.AppendLine("            <div class=\"section-content\">");
        sb.AppendLine($"                <div class=\"exception-type\">{EscapeHtml(exception.GetType().FullName ?? exception.GetType().Name)}</div>");
        sb.AppendLine($"                <div class=\"exception-message\">{EscapeHtml(exception.Message)}</div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");

        // Stack Trace Section
        if (includeStackTrace && !string.IsNullOrEmpty(exception.StackTrace))
        {
            sb.AppendLine("        <div class=\"section\">");
            sb.AppendLine("            <div class=\"section-header\">📋 Stack Trace</div>");
            sb.AppendLine("            <div class=\"section-content\">");
            sb.AppendLine($"                <div class=\"stack-trace\">{FormatStackTrace(exception.StackTrace)}</div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
        }

        // Request Information Section
        sb.AppendLine("        <div class=\"section\">");
        sb.AppendLine("            <div class=\"section-header\">🌐 Request Information</div>");
        sb.AppendLine("            <div class=\"section-content\">");
        sb.AppendLine("                <div class=\"info-grid\">");
        sb.AppendLine($"                    <div class=\"info-label\">Path:</div>");
        sb.AppendLine($"                    <div class=\"info-value\">{EscapeHtml(context.Request.Path + context.Request.QueryString)}</div>");
        sb.AppendLine($"                    <div class=\"info-label\">Method:</div>");
        sb.AppendLine($"                    <div class=\"info-value\">{EscapeHtml(context.Request.Method)}</div>");
        sb.AppendLine($"                    <div class=\"info-label\">Protocol:</div>");
        sb.AppendLine($"                    <div class=\"info-value\">{EscapeHtml(context.Request.Protocol)}</div>");
        sb.AppendLine($"                    <div class=\"info-label\">Content Type:</div>");
        sb.AppendLine($"                    <div class=\"info-value\">{EscapeHtml(context.Request.ContentType ?? "N/A")}</div>");
        sb.AppendLine("                </div>");
        
        // Request Headers (collapsible) - Sensitive headers are filtered
        sb.AppendLine("                <button class=\"toggle-button\" onclick=\"toggleSection('request-headers')\">Show/Hide Request Headers</button>");
        sb.AppendLine("                <div id=\"request-headers\" class=\"collapsible-content\">");
        foreach (var header in context.Request.Headers)
        {
            // Filter sensitive headers
            var headerValue = IsSensitiveHeader(header.Key) 
                ? "[REDACTED]" 
                : string.Join(", ", header.Value.ToArray());
            sb.AppendLine($"                    <div class=\"data-item\"><strong>{EscapeHtml(header.Key)}:</strong> {EscapeHtml(headerValue)}</div>");
        }
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");

        // Inner Exceptions
        if (exception.InnerException != null)
        {
            sb.AppendLine("        <div class=\"section\">");
            sb.AppendLine("            <div class=\"section-header\">🔗 Inner Exceptions</div>");
            sb.AppendLine("            <div class=\"section-content\">");
            
            var innerEx = exception.InnerException;
            var level = 1;
            while (innerEx != null)
            {
                sb.AppendLine("                <div class=\"inner-exception\">");
                sb.AppendLine($"                    <div class=\"inner-exception-title\">Inner Exception Level {level}: {EscapeHtml(innerEx.GetType().Name)}</div>");
                sb.AppendLine($"                    <div class=\"exception-message\">{EscapeHtml(innerEx.Message)}</div>");
                if (includeStackTrace && !string.IsNullOrEmpty(innerEx.StackTrace))
                {
                    sb.AppendLine($"                    <button class=\"toggle-button\" onclick=\"toggleSection('inner-stack-{level}')\">Show/Hide Stack Trace</button>");
                    sb.AppendLine($"                    <div id=\"inner-stack-{level}\" class=\"collapsible-content\">");
                    sb.AppendLine($"                        <div class=\"stack-trace\">{FormatStackTrace(innerEx.StackTrace)}</div>");
                    sb.AppendLine("                    </div>");
                }
                sb.AppendLine("                </div>");
                
                innerEx = innerEx.InnerException;
                level++;
            }
            
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
        }

        // Exception Data
        if (exception.Data.Count > 0)
        {
            sb.AppendLine("        <div class=\"section\">");
            sb.AppendLine("            <div class=\"section-header\">💾 Exception Data</div>");
            sb.AppendLine("            <div class=\"section-content\">");
            foreach (var key in exception.Data.Keys)
            {
                var value = exception.Data[key];
                sb.AppendLine($"                <div class=\"data-item\"><strong>{EscapeHtml(key?.ToString() ?? "null")}:</strong> {EscapeHtml(value?.ToString() ?? "null")}</div>");
            }
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
        }

        sb.AppendLine("        <div class=\"footer\">");
        sb.AppendLine("            <p>OrkinosaiCMS Development Error Harness</p>");
        sb.AppendLine("            <p>Timestamp: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") + "</p>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private string FormatStackTrace(string stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
            return string.Empty;

        // Simple formatting to highlight file paths and line numbers
        var formatted = EscapeHtml(stackTrace);
        
        // Highlight file paths (e.g., in C:\path\to\file.cs:line 123)
        formatted = System.Text.RegularExpressions.Regex.Replace(
            formatted,
            @"(in\s+)([^\s:]+)(:\s*line\s+)(\d+)",
            "<span class=\"file-path\">$1$2</span><span class=\"line-number\">$3$4</span>");

        return formatted;
    }

    private static string EscapeHtml(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    private static bool IsSensitiveHeader(string headerName)
    {
        var sensitiveHeaders = new[] 
        { 
            "authorization", 
            "cookie", 
            "set-cookie", 
            "x-api-key", 
            "x-auth-token",
            "x-csrf-token",
            "www-authenticate"
        };
        
        return sensitiveHeaders.Contains(headerName.ToLowerInvariant());
    }
}
