﻿// <auto-generated/>
#pragma warning disable 1591
namespace Test
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "x:\dir\subdir\Test\TestComponent.cshtml"
using Microsoft.AspNetCore.Components.Rendering;

#line default
#line hidden
#nullable disable
    public partial class TestComponent : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 3 "x:\dir\subdir\Test\TestComponent.cshtml"
       
    void RenderChildComponent(RenderTreeBuilder __builder)
    {
        var output = string.Empty;
        if (__builder == null) output = "Builder is null!";
        else output = "Builder is not null!";

#line default
#line hidden
#nullable disable
        __builder.OpenElement(0, "p");
        __builder.AddContent(1, "Output: ");
#nullable restore
#line (9,21)-(9,27) 24 "x:\dir\subdir\Test\TestComponent.cshtml"
__builder.AddContent(2, output);

#line default
#line hidden
#nullable disable
        __builder.CloseElement();
#nullable restore
#line 10 "x:\dir\subdir\Test\TestComponent.cshtml"
    }

#line default
#line hidden
#nullable disable
    }
}
#pragma warning restore 1591
