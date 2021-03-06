using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml;


namespace TileContent
{
    [ContentProcessor(DisplayName = "Script Processor")]
    public class ScriptProcessor : ContentProcessor<XmlDocument, ScriptContent>
    {
        public override ScriptContent Process(XmlDocument input, ContentProcessorContext context)
        {
            ScriptContent script = new ScriptContent();

            XmlNodeList conversations = input.GetElementsByTagName("Conversation");

            foreach (XmlNode node in conversations)
            {
                ConversationContent c = new ConversationContent();
                c.Name = node.Attributes["Name"].Value;
                c.Text = node.FirstChild.InnerText;

                foreach (XmlNode handlerNode in node.LastChild.ChildNodes)
                {
                    ConversationHandlerContent h = new ConversationHandlerContent();
                    h.Caption = handlerNode.Attributes["Caption"].Value;

                    string action = handlerNode.Attributes["Action"].Value;

                    if (action.Contains(":"))
                    {
                        string[] actionSplit = action.Split(':');
                        h.Action = actionSplit[0];
                        h.ActionParameters = (object[])actionSplit[1].Split(',');
                    }
                    else
                    {
                        h.Action = action;
                        h.ActionParameters = null;
                    }
                    c.Handlers.Add(h);
                }
                script.Conversations.Add(c);
            }
            return script;
        }
    }
}