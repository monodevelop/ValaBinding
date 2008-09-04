//
// StructureNodeBuilder.cs
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;

using Mono.Addins;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Pads;
using MonoDevelop.Core.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Components;

using MonoDevelop.ValaBinding.Parser;

namespace MonoDevelop.ValaBinding.Navigation
{
	public class StructureNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType {
			get { return typeof(Structure); }
		}
		
		public override Type CommandHandlerType {
			get { return typeof(LanguageItemCommandHandler); }
		}
		
		public override string GetNodeName (ITreeNavigator thisNode, object dataObject)
		{
			return ((Structure)dataObject).Name;
		}
		
		public override void BuildNode (ITreeBuilder treeBuilder,
		                                object dataObject,
		                                ref string label,
		                                ref Gdk.Pixbuf icon,
		                                ref Gdk.Pixbuf closedIcon)
		{
			Structure s = (Structure)dataObject;
				
			label = s.Name;
			
			switch (s.Access)
			{
			case AccessModifier.Public:
				icon = Context.GetIcon (Stock.Struct);
				break;
			case AccessModifier.Protected:
				icon = Context.GetIcon (Stock.ProtectedStruct);
				break;
			case AccessModifier.Private:
				icon = Context.GetIcon (Stock.PrivateStruct);
				break;
			}
		}
		
		public override void BuildChildNodes (ITreeBuilder treeBuilder, object dataObject)
		{
			ValaProject p = treeBuilder.GetParentDataItem (typeof(ValaProject), false) as ValaProject;
			
			if (p == null) return;
			
			bool publicOnly = treeBuilder.Options["PublicApiOnly"];
			ProjectInformation info = ProjectInformationManager.Instance.Get (p);
			
			Structure thisStruct = (Structure)dataObject;
			
			// Classes
			foreach (Class c in info.Classes)
				if (c.Parent != null && c.Parent.Equals (thisStruct) && (!publicOnly || c.Access == AccessModifier.Public))
					treeBuilder.AddChild (c);
			
			// Structures
			foreach (Structure s in info.Structures)
				if (s.Parent != null && s.Parent.Equals (thisStruct) && (!publicOnly || s.Access == AccessModifier.Public))
					treeBuilder.AddChild (s);
			
			// Unions
			foreach (Union u in info.Unions)
				if (u.Parent != null && u.Parent.Equals (thisStruct) && (!publicOnly || u.Access == AccessModifier.Public))
					treeBuilder.AddChild (u);
			
			// Enumerations
			foreach (Enumeration e in info.Enumerations)
				if (e.Parent != null && e.Parent.Equals (thisStruct) && (!publicOnly || e.Access == AccessModifier.Public))
					treeBuilder.AddChild (e);
			
			// Typedefs
			foreach (Typedef t in info.Typedefs)
				if (t.Parent != null && t.Parent.Equals (thisStruct) && (!publicOnly || t.Access == AccessModifier.Public))
					treeBuilder.AddChild (t);
			
			// Functions
			foreach (Function f in info.Functions)
				if (f.Parent != null && f.Parent.Equals (thisStruct) && (!publicOnly || f.Access == AccessModifier.Public))
					treeBuilder.AddChild (f);
			
			// Members
			foreach (Member m in info.Members)
				if (m.Parent != null && m.Parent.Equals (thisStruct) && (!publicOnly || m.Access == AccessModifier.Public))
					treeBuilder.AddChild (m);
		}
		
		public override bool HasChildNodes (ITreeBuilder builder, object dataObject)
		{
			return true;
		}
		
		public override int CompareObjects (ITreeNavigator thisNode, ITreeNavigator otherNode)
		{
			if (otherNode.DataItem is Class)
				return 1;
			else
				return -1;
		}
	}
}
