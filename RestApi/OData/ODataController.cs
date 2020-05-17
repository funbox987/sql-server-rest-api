﻿// Copyright (c) Jovan Popovic. All Rights Reserved.
// Licensed under the BSD License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TSql.RestApi;

namespace TSql.OData
{
    /// <summary>
    /// Controller class that should be used to expose OData REST API with minimal metadata.
    /// </summary>
    public abstract class ODataController : Controller
    {
        /// <summary>
        /// Array of table specifications that describe tables that should be exposed.
        /// </summary>
        public abstract TableSpec[] TableSpec { get; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string MetadataPath { get; } = "$metadata";

        /// <summary>
        /// Url that will be placed in XML metadata.
        /// </summary>
        public virtual string MetadataUrl
        {
            get {
                return this.Request.Scheme + "://" + this.Request.Host + "/" + this.MetadataPath;
            }
        }

        /// <summary>
        /// Namespace of the model classes that will be generated by client using OData metadata.
        /// Default is <ControllerName>.Models.
        /// </summary>
        public virtual string ModelNamespace
        {
            get
            {
                return this.ControllerContext.ActionDescriptor.ControllerName + ".Models";
            }
        }

        protected readonly TSqlCommand DbCommand;
        
        public ODataController(TSqlCommand command)
        {
            this.DbCommand = command;
        }

        /// <summary>
        /// Returns Service document that describes the entities in the service.
        /// <see cref="https://services.odata.org/TripPinRESTierService/(S(0rl14bktppv5tp5hy3obiftc))/"/>
        /// </summary>
        /// <returns>Service document JOSN content that describes OData services.</returns>
        [HttpGet]
        public ActionResult Index()
        {
#if (!NETSTANDARD1_6 && !NETCOREAPP1_0 && !NETCOREAPP1_1)
            return this.GetODataServiceDocumentJsonV4(this.TableSpec, MetadataAction: ODataMetadataXml);
#else
            return this.GetODataServiceDocumentJsonV4(this.TableSpec, "$metadata");
#endif
        }

        /// <summary>
        /// Returns metadata information about the entities in the OData service.
        /// </summary>
        /// <returns>XML document containing metadata describtion of each entity described using <c>TableSpec</c>.</returns>
        [HttpGet("[controller]/$metadata")]
        public ActionResult ODataMetadataXml()
        {
            return this.GetODataMetadataXmlV4(this.TableSpec);
        }
    }
}
