namespace KnockoutMvc
{
    using System;
    using System.Web.Mvc;
    using System.IO;

    public abstract class KnockoutRegionContext<TModel> : KnockoutContext<TModel>, IDisposable
    {
        public KnockoutRegionContext(KnockoutContext<TModel> context)
            : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.writer = this.htmlHelper.ViewContext.Writer;
            this.InStack = true;
        }

        public bool InStack { get; set; }

        private bool disposed;
        private readonly TextWriter writer;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                this.WriteEnd(writer);
                if (this.InStack)
                {
                    this.ContextStack.RemoveAt(this.ContextStack.Count - 1);
                }

                disposed = true;
            }
        }

        public abstract void WriteStart(TextWriter writer);
        protected abstract void WriteEnd(TextWriter writer);
    }
}
