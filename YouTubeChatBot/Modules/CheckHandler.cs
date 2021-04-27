using System;
using System.Collections.Generic;
using System.Text;
using YouTubeChatBot.Models;
using YouTubeChatBot.Interfaces;
using YouTubeChatBot.Services;
using System.IO;

namespace YouTubeChatBot.Modules
{
    class CheckHandler : BaseYTModule
    {
        protected override string savefileformat => "Stream[{0}]Checks";
        public CheckHandler(FileService fileService, SerializeService serializeService, ConfigurationService configurationService)
            : base(fileService, serializeService, configurationService.CheckConfigure) { }

        //public override void Execute(YTMessageResponse param)
        //{
        //    var removeLength = Prefix.Length + 2;
        //    var parse = param.Context.Remove(0, removeLength < param.Context.Length ? removeLength : param.Context.Length);
        //    var model = new CheckSaveModel
        //    {
        //        DateTime = param.UtcTime,
        //        Comment = parse,
        //        UserName = param.UserName,
        //        SecondFromStreamStart = (long)(param.UtcTime - param.StartStreamTime).TotalSeconds
        //    };
        //    var formatDate = param.StartStreamTime.ToString().Replace(' ', '_').Replace('.', '-').Replace(':', '-');
        //    SaveFile(model, formatDate);
        //}
    }
}
