#pragma checksum "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Songs\Delete.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b9c7768432980628e8a4dc32c858ab5cc1f8edc4"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Songs_Delete), @"mvc.1.0.view", @"/Views/Songs/Delete.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Songs/Delete.cshtml", typeof(AspNetCore.Views_Songs_Delete))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b9c7768432980628e8a4dc32c858ab5cc1f8edc4", @"/Views/Songs/Delete.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"02cc697e419f53a234b34bb840b9297d7aa1010c", @"/Views/_ViewImports.cshtml")]
    public class Views_Songs_Delete : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VodManageSystem.Models.DataModels.Song>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(47, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 3 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Songs\Delete.cshtml"
  
    ViewData["Title"] = "Delete Song";
    ViewBag.Action = "Delete";

#line default
#line hidden
            BeginContext(128, 32, true);
            WriteLiteral("\r\n<h3 style=\"text-align:center\">");
            EndContext();
            BeginContext(161, 17, false);
#line 8 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Songs\Delete.cshtml"
                         Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(178, 9, true);
            WriteLiteral("</h3>\r\n\r\n");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(205, 2, true);
                WriteLiteral("\r\n");
                EndContext();
#line 11 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Songs\Delete.cshtml"
      
        await Html.RenderPartialAsync("~/Views/Songs/_SongValidationScriptsPartial.cshtml");
    

#line default
#line hidden
            }
            );
            BeginContext(319, 2, true);
            WriteLiteral("\r\n");
            EndContext();
            BeginContext(322, 59, false);
#line 16 "C:\VisualStudio\Smile_SoftwareSolution\VodManageSystem\Views\Songs\Delete.cshtml"
Write(await Html.PartialAsync("_SongOneRecordForm.cshtml", Model));

#line default
#line hidden
            EndContext();
            BeginContext(381, 4, true);
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VodManageSystem.Models.DataModels.Song> Html { get; private set; }
    }
}
#pragma warning restore 1591
