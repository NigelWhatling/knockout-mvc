namespace KnockoutMvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class KnockoutJsonResult : ActionResult
    {
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public JsonRequestBehavior JsonRequestBehavior { get; set; }
        public ReferenceLoopHandling ReferenceLoopHandling { get; set; }
             
        public ModelStateDictionary ModelState { get; set; }

        [JsonProperty("__Data", Order = 1)]
        public object Data { get; set; }

        [JsonProperty("__Message", Order = 2)]
        public string Message { get; set; }

        [JsonProperty("__ModelErrors", Order = 3)]
        public object[] ModelErrors
        {
            get
            {
                if (this.ModelState == null)
                {
                    return null;
                }

                List<object> errors = new List<object>();
                foreach (KeyValuePair<string, ModelState> state in this.ModelState.Where(e => e.Value.Errors.Count > 0))
                {
                    state.Value.Errors.ToList().ForEach((e) => { errors.Add(new { Key = state.Key, Message = e.ErrorMessage }); });
                }

                return errors.ToArray();
            }
        }

        [JsonProperty("__Redirect", Order = 4)]
        public string Redirect { get; set; }

        public bool ShouldSerializeMessage()
        {
            return this.Message != null;
        }

        public bool ShouldSerializeModelErrors()
        {
            return this.ModelErrors != null && this.ModelErrors.Count() > 0;
        }

        public bool ShouldSerializeRedirect()
        {
            return this.Redirect != null;
        }

        public KnockoutJsonResult()
        {
            this.ContentEncoding = Encoding.UTF8;
            this.ContentType = "application/json";
            this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            this.ReferenceLoopHandling = ReferenceLoopHandling.Error;
        }

        public KnockoutJsonResult(object data)
            : this()
        {
            this.Data = data;
        }

        public KnockoutJsonResult(object data, string message)
            : this(data)
        {
            this.Message = message;
        }

        public KnockoutJsonResult(object data, ModelStateDictionary modelState)
            : this(data)
        {
            this.ModelState = modelState;
        }

        public KnockoutJsonResult(object data, string message, ModelStateDictionary modelState)
            : this(data, message)
        {
            this.ModelState = modelState;
        }

        public KnockoutJsonResult(string url)
            : this()
        {
            this.Redirect = url;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && this.JsonRequestBehavior == JsonRequestBehavior.DenyGet)
            {
                throw new InvalidOperationException("GET request denied.");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = this.ContentType;
            response.ContentEncoding = this.ContentEncoding;

            response.Write(JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = this.ReferenceLoopHandling }));
        }
    }
}
