using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TileContent
{
    [ContentTypeWriter]
    public class ScriptWriter : ContentTypeWriter<ScriptContent>
    {
        protected override void Write(ContentWriter output, ScriptContent value)
        {
            output.Write(value.Conversations.Count);
            foreach (ConversationContent c in value.Conversations)
                output.WriteObject(c);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Enders2.ScriptReader, Enders2.0";
        }
    }

    [ContentTypeWriter]
    public class ConversationWriter : ContentTypeWriter<ConversationContent>
    {
        protected override void Write(ContentWriter output, ConversationContent value)
        {
            output.Write(value.Name);
            output.Write(value.Text);

            output.Write(value.Handlers.Count);
                foreach(ConversationHandlerContent c in value.Handlers)
                    output.WriteObject(c);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {

            return "Enders2.ConversationReader, Enders2.0";
        }
    }

    [ContentTypeWriter]
    public class ConversationHandlerWriter : ContentTypeWriter<ConversationHandlerContent>
    {
        protected override void Write(ContentWriter output, ConversationHandlerContent value)
        {
            output.Write(value.Caption);
            output.Write(value.Action);
            output.WriteObject(value.ActionParameters);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {

            return "Enders2.ConversationHandlerReader, Enders2.0";
        }
    }
}
