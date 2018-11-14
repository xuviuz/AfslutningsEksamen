using Monosoft.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;

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

            if (true) // cc.IsServerSideFunctionsAdmin
            {
                if (wrapper.MessageData != null)
                {
                    switch (operation)
                    {
                        case "create":
                            var createFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            // string createResult = Compiler.CreateDll(createFuncDef.Name, createFuncDef.FunctionData);
                            string createResult = "CREATE result will be later";
                            //eventdata = new Common.DTO.EventDTO(res, wrapper.Clientid, wrapper.Messageid);
                            //Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.Scope, eventdata);
                            
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
                            string sharpFunctionToDelet = deleteResult.FunctionData;
                            break;
                        case "update":
                            // check if function isExist
                            Console.WriteLine("update");

                            break;
                        case "read":
                            // check if function isExist
                            Console.WriteLine("read");

                            break;
                        case "run":
                            var runFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            //object[] parameters = 
                            //var operationResult = Compiler.RunDll(runFuncDef.Name, runFuncDef.FunctionData);
                            var operationResult = "RUN will be later";
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
