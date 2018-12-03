﻿#region License
/*
Copyright © 2014-2018 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using Amdocs.Ginger.Repository;
using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Ginger.Reports;

using Amdocs.Ginger;
using amdocs.ginger.GingerCoreNET;
using Ginger.Run.RunsetActions;
using GingerCoreNET.ReporterLib;
using Amdocs.Ginger.Common.InterfacesLib;

namespace Ginger.Run.RunSetActions
{
    public class RunSetActionHTMLReport : RunSetActionBase
    {
        public new static class Fields
        {
            public static string HTMLReportFolderName = "HTMLReportFolderName";
            public static string isHTMLReportFolderNameUsed = "isHTMLReportFolderNameUsed";
            public static string isHTMLReportPermanentFolderNameUsed = "isHTMLReportPermanentFolderNameUsed";
            public static string selectedHTMLReportTemplateID = "selectedHTMLReportTemplateID";
        }

        public override List<RunSetActionBase.eRunAt> GetRunOptions()
        {
            List<RunSetActionBase.eRunAt> list = new List<RunSetActionBase.eRunAt>();
            list.Add(RunSetActionBase.eRunAt.ExecutionEnd);
            return list;
        }

        public override bool SupportRunOnConfig
        {
            get { return true; }
        }

        [IsSerializedForLocalRepository]
        public string HTMLReportFolderName { get; set; }

        [IsSerializedForLocalRepository]
        public int selectedHTMLReportTemplateID { get; set; }

        [IsSerializedForLocalRepository]
        public bool isHTMLReportFolderNameUsed { get; set; }

        [IsSerializedForLocalRepository]
        public bool isHTMLReportPermanentFolderNameUsed { get; set; }

        public override void Execute(IReportInfo RI)
        {
            string reportsResultFolder = string.Empty;
            HTMLReportsConfiguration currentConf = WorkSpace.Solution.HTMLReportsConfigurationSetList.Where(x => (x.IsSelected == true)).FirstOrDefault();
            if (WorkSpace.RunsetExecutor.RunSetConfig.RunsetExecLoggerPopulated)
            {
                string runSetFolder = string.Empty;
                if (WorkSpace.RunsetExecutor.RunSetConfig.LastRunsetLoggerFolder != null)
                {
                    runSetFolder = WorkSpace.RunsetExecutor.RunSetConfig.LastRunsetLoggerFolder;
                    AutoLogProxy.UserOperationStart("Online Report");
                }
                else
                {
                    runSetFolder = ExecutionLogger.GetRunSetLastExecutionLogFolderOffline();
                    AutoLogProxy.UserOperationStart("Offline Report");
                }
                if (!string.IsNullOrEmpty(selectedHTMLReportTemplateID.ToString()))
                {
                    if ((isHTMLReportFolderNameUsed) && (HTMLReportFolderName != null) && (HTMLReportFolderName != string.Empty))
                    {
                        ObservableList<HTMLReportConfiguration> HTMLReportConfigurations = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<HTMLReportConfiguration>();
                        reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder),
                                                                                                                                false,
                                                                                                                                HTMLReportConfigurations.Where(x => (x.ID == selectedHTMLReportTemplateID)).FirstOrDefault(),
                                                                                                                                HTMLReportFolderName + "\\" + System.IO.Path.GetFileName(runSetFolder),
                                                                                                                                isHTMLReportPermanentFolderNameUsed, currentConf.HTMLReportConfigurationMaximalFolderSize);
                    }
                    else
                    {
                        ObservableList<HTMLReportConfiguration> HTMLReportConfigurations = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<HTMLReportConfiguration>();
                        reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder),
                                                                                                                                false,
                                                                                                                                HTMLReportConfigurations.Where(x => (x.ID == selectedHTMLReportTemplateID)).FirstOrDefault(),
                                                                                                                                null,
                                                                                                                                isHTMLReportPermanentFolderNameUsed);
                    }
                }
                else
                {
                    reportsResultFolder = Ginger.Reports.GingerExecutionReport.ExtensionMethods.CreateGingerExecutionReport(new ReportInfo(runSetFolder),
                                                                                                                                false,
                                                                                                                                null,
                                                                                                                                null,
                                                                                                                                isHTMLReportPermanentFolderNameUsed);
                }
            }
            else
            {
                Errors = "In order to get HTML report, please, perform executions before";
                Reporter.CloseGingerHelper();
                Status = RunSetActionBase.eRunSetActionStatus.Failed;
                return;
            }
        }

        public override string GetEditPage()
        {
            //RunSetActionHTMLReportEditPage p = new RunSetActionHTMLReportEditPage(this);
            return "RunSetActionHTMLReportEditPage";
        }

        public override void PrepareDuringExecAction(ObservableList<IGingerRunner> Gingers)
        {
            throw new NotImplementedException();
        }

        public override string Type { get { return "Produce HTML Report"; } }
    }
}
