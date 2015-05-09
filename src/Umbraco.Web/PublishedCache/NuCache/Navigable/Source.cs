﻿using System;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Xml.XPath;

namespace Umbraco.Web.PublishedCache.NuCache.Navigable
{
    class Source : INavigableSource
    {
        private readonly INavigableData _data;
        private readonly bool _preview;
        private readonly RootContent _root;

        public Source(INavigableData data, bool preview)
        {
            _data = data;
            _preview = preview;

            var contentAtRoot = data.GetAtRoot(preview);
            _root = new RootContent(contentAtRoot.Select(x => x.Id));
        }

        public INavigableContent Get(int id)
        {
            // wrap in a navigable content

            var content = _data.GetById(_preview, id);
            if (content == null) return null;

            // content may be a strongly typed model, have to unwrap first

            PublishedContentWrapped wrapped;
            while ((wrapped = content as PublishedContentWrapped) != null)
                content = wrapped.Unwrap();
            var published = content as IPublishedContentOrMedia;
            if (published == null)
                throw new InvalidOperationException("Innermost content is not IPublishedContentOrMedia.");
            return new NavigableContent(published);
        }

        public int LastAttributeIndex
        {
            get { return NavigableContentType.BuiltinProperties.Length - 1; }
        }

        public INavigableContent Root
        {
            get { return _root; }
        }
    }
}