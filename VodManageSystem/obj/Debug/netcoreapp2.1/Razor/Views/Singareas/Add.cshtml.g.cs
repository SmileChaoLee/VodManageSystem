#pragma checksum "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singareas\Add.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b0619a66885b74f19e6ba4fd7c931e71b81f06c8"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Singareas_Add), @"mvc.1.0.view", @"/Views/Singareas/Add.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Singareas/Add.cshtml", typeof(AspNetCore.Views_Singareas_Add))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\_ViewImports.cshtml"
using VodManageSystem;

#line default
#line hidden
#line 2 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\_ViewImports.cshtml"
using VodManageSystem.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b0619a66885b74f19e6ba4fd7c931e71b81f06c8", @"/Views/Singareas/Add.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Singareas_Add : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Singarea>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(51, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singareas\Add.cshtml"
  
    ViewData["Title"] = "Add Singer Areas";
    ViewBag.Action = "Add";

#line default
#line hidden
            BeginContext(134, 33, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center;\">");
            EndContext();
            BeginContext(168, 17, false);
#line 8 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singareas\Add.cshtml"
                          Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(185, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(212, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singareas\Add.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Singareas/_SingareaValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(334, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(337, 63, false);
#line 16 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Singareas\Add.cshtml"
Write(await Html.PartialAsync("_SingareaOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(400, 4, true);
            WriteLiteral("\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VodManageSystem.Models.DataModels.Singarea> Html { get; private set; }
    }
}
#pragma warning restore 1591
