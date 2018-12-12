﻿using Monosoft.Common.DTO;
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
            FunctionHandler functionHandler = new FunctionHandler();

            if (true) // cc.IsServerSideFunctionsAdmin
            {
                if (wrapper.MessageData != null)
                {
                    
                    switch (operation)
                    {
                        case "create":
                            var createFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string createResult = functionHandler.CreateFunction(createFuncDef);

                            eventdata = new Common.DTO.EventDTO(createResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionCreated, eventdata);

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
                            var deleteFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);
                            string deleteResult = functionHandler.DeleteFunction(deleteFuncDef.Name);

                            eventdata = new Common.DTO.EventDTO(deleteResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionDeleted, eventdata);

                            if (deleteResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, deleteResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, deleteResult);
                            }

                            break;
                        case "update":
                            var updateFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string updateResult = functionHandler.UpdateFunction(updateFuncDef);

                            eventdata = new Common.DTO.EventDTO(updateResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionUpdated, eventdata);

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

                            string readResult = functionHandler.ReadFunction(readFunc.Name);

                            eventdata = new Common.DTO.EventDTO(readResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionRead, eventdata);

                            if (readResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, readResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, readResult);
                            }

                            break;
                        case "readall":

                            var readallFunc = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            string readallResult = functionHandler.ReadAllFunctions(readallFunc.Name);

                            eventdata = new Common.DTO.EventDTO(readallResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionReadAll, eventdata);

                            if (readallResult != null)
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "OK" } }, readallResult);
                            }
                            else
                            {
                                return ReturnMessageWrapper.CreateResult(true, wrapper, new System.Collections.Generic.List<LocalizedString>() { new LocalizedString() { Lang = "en", Text = "Missing rights" } }, readallResult);
                            }


                            break;
                        case "run":
                            var runFuncDef = Common.DTO.MessageWrapperHelper<DTO.FunctionDefinitions>.GetData(wrapper);

                            object[] parameters = functionHandler.ConvertToObjectArray(runFuncDef.FunctionData);
                            var operationResult = functionHandler.RunFunction(runFuncDef.Name, parameters);

                            eventdata = new Common.DTO.EventDTO(operationResult, wrapper.Clientid, wrapper.Messageid);
                            Common.MessageQueue.EventClient.Instance.RaiseEvent(GlobalValues.RouteFunctionRun, eventdata);

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
