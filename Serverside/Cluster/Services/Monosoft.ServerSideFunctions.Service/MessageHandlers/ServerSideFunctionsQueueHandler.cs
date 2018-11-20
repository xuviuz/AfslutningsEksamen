using Monosoft.Common.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    /// <summary>
    /// Message handler for ServerSideFunctions messages
    /// </summary>
    class ServerSideFunctionsQueueHandler
    {
        /// <summary>
        /// The message handler logic for organisation messages
        /// </summary>
        /// <param name="topicparts">topic parts</param>
        /// <param name="wrapper">wrapper object</param>
        /// <returns>resulting object or null</returns>
        public static ReturnMessageWrapper HandleMessage(string[] topicparts, Common.DTO.MessageWrapper wrapper)
        {
            //CallContext cc = new CallContext(wrapper.OrgContext, new Common.DTO.Token() { Tokenid = wrapper.UserContextToken, Scope = GlobalValues.Scope }, wrapper.IssuedDate);
            var operation = topicparts[1];
            Common.DTO.EventDTO eventdata = null;
            Compiler compiler = new Compiler();

            if (true) // cc.IsServerSideFunctionsAdmin
            {
                if (wrapper.MessageData != null)
                {
                    switch (operation)
                    {
                        case "create":
                            var createFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string createResult = compiler.CreateDll(createFuncDef.Name, createFuncDef.FunctionData);

                            if (createResult != "Error whole creating" + createFuncDef.Name)
                            {
                                File.WriteAllText(Directory.GetCurrentDirectory() + @"\Dller\" + createFuncDef.Name + @"\" + createFuncDef.Name + ".json", JsonConvert.SerializeObject(createFuncDef));

                            }
                            //string createResult = "CREATE result will be later";
                            //eventdata = new Common.DTO.EventDTO(createResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.Scope, eventdata);

                            if (createResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, createResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, createResult);
                            }

                            break;
                        case "delete":
                            var deleteResult = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string deletedResult = "ERROR!";
                            if (compiler.DeleteFunc(deleteResult.Name) == true)
                            {
                                deletedResult = deleteResult.Name.ToUpper() + " WAS DELETED!";
                            }
                            else
                            {
                                deletedResult = "FILE DOES NOT EXIST! MAYBE THERE WAS A TYPO?";
                            }
                            if (deletedResult != "ERROR!")
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, deletedResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, deletedResult);
                            }

                            break;
                        case "update":
                            var updateFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string updateResult = compiler.UpdateDLL(updateFuncDef.Name, updateFuncDef.FunctionData);
                            //string createResult = "CREATE result will be later";
                            //eventdata = new Common.DTO.EventDTO(createResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.Scope, eventdata);

                            if (updateResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, updateResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, updateResult);
                            }

                            break;
                        case "read":

                            var readFunc = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string readResult = compiler.ReadDll(readFunc.Name);

                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.Scope, eventdata);

                            if (readResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, readResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, readResult);
                            }

                            break;
                        case "run":
                            var runFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            object[] parameters = compiler.ConvertToObjectArray(runFuncDef.FunctionData);
                            var operationResult = compiler.RunDll(runFuncDef.Name, parameters);
                            //var operationResult = "RUN will be later";
                            //eventdata = new Common.DTO.EventDTO(res, wrapper.Clientid, wrapper.Messageid);
                            //Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.Scope, eventdata);

                            if (operationResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, operationResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, operationResult);
                            }
                            break;
                        default: /*log error event*/
                            Common.MessageQueue.Diagnostics.Instance.LogEvent("Unknow topic for ServerSideFunctions.", operation + " is unknown", Common.DTO.Severity.Information, System.Guid.Empty);
                            break;
                    }
                }
            }
            return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "unknown situation" } }, null);
        }
    }
}
