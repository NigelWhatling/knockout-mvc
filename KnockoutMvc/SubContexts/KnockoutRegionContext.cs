namespace KnockoutMvc
{
    using System;
    using System.Web.Mvc;
    using System.IO;

    public abstract class KnockoutRegionContext<TModel> : KnockoutContext<TModel>, IDisposable
    {
        public KnockoutRegionContext(ViewContext viewContext)
            : base(viewContext)
        {
            if (viewContext == null)
                throw new ArgumentNullException("viewContext");
            writer = viewContext.Writer;
            InStack = true;
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
