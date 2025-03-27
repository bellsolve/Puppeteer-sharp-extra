﻿using System.Reflection;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PuppeteerExtraSharp.Plugins.ExtraStealth.Evasions
{
    public class SourceUrl : PuppeteerExtraPlugin
    {
        public SourceUrl() : base("SourceUrl")
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task OnPageCreated(IPage page)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var mainWordProperty =
                page.MainFrame.GetType().GetProperty("MainWorld", BindingFlags.NonPublic
                                                                  | BindingFlags.Public | BindingFlags.Instance);
            var mainWordGetters = mainWordProperty.GetGetMethod(true);
            
            page.Load += async (_, _) =>
            {
                var mainWord = mainWordGetters.Invoke(page.MainFrame, null);
                var contextField = mainWord.GetType()
                    .GetField("_contextResolveTaskWrapper", BindingFlags.NonPublic | BindingFlags.Instance);
                if (contextField is not null)
                {
                    var context = (TaskCompletionSource<ExecutionContext>) contextField.GetValue(mainWord);
                    var execution = await context.Task;
                    var suffixField = execution.GetType()
                        .GetField("_evaluationScriptSuffix", BindingFlags.NonPublic | BindingFlags.Instance);
                    suffixField?.SetValue(execution, "//# sourceURL=''");
                }
            };
        }
    }
}