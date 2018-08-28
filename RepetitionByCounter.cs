using System;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Data;

namespace Honoka.SmallArticle.Activities
{
    [Description("Simple Repitation Loop with Counter")]
    [DisplayName("Repetition By Counter")]
    public class RepetitionByCounter : NativeActivity
    {
        [Browsable(false)]
        public ActivityAction<Int32> Loop { get; set; }

        [Browsable(false)]
        public InArgument<Int32> LoopValue { get; set; }

        /// <summary>
        /// Start Value
        /// </summary>
        [Description("value at first execution")]
        [RequiredArgument]
        [Category("Settings")]
        public InArgument<Int32> From { get; set; } = 1;

        /// <summary>
        /// End Value
        /// </summary>
        [Description("value at final execution (Includes this value)")]
        [RequiredArgument]
        [Category("Settings")]
        public InArgument<Int32> To { get; set; } = 10;

        /// <summary>
        /// Step
        /// </summary>
        [Description("Addition value on each time")]
        [RequiredArgument]
        [Category("Settings")]
        public InArgument<Int32> Addition { get; set; } = 1;

        
        /// <summary>
        /// Initialize
        /// </summary>
        public RepetitionByCounter()
        {
            Loop = new ActivityAction<int>()
            {
                Argument = new DelegateInArgument<Int32>("counter")
                , Handler = new System.Activities.Statements.Sequence() { DisplayName = "Do" }
            };
        }

        
        /// <summary>
        /// Execution Main
        /// </summary>
        /// <param name="context"></param>
        protected override void Execute(NativeActivityContext context)
        {
            if (Addition.Get(context) == 0) throw new ArgumentException("[Addition] must not Zero");
            if (Addition.Get(context) > 0 ^ From.Get(context) < To.Get(context)) throw new ArgumentException("Invalid [From] and [To]");

            if(Loop != null)
            {
                LoopValue.Set(context, From.Get(context));
                context.ScheduleAction<Int32>(Loop, LoopValue.Get(context), LoopExecution, null);
            }

        }

        /// <summary>
        /// Executes on Each Loop
        /// </summary>
        /// <param name="context"></param>
        /// <param name="completedInstance"></param>
        private void LoopExecution(NativeActivityContext context, ActivityInstance completedInstance)
        {
            LoopValue.Set(context, LoopValue.Get(context) + Addition.Get(context));

            if( (Addition.Get(context) > 0) ? (To.Get(context) >= LoopValue.Get(context)) : (To.Get(context) <= LoopValue.Get(context)))
            {
                context.ScheduleAction<Int32>(Loop, LoopValue.Get(context), LoopExecution, null);
            }
        }
    }
}
