﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]
#region EDM Relationship Metadata

[assembly: EdmRelationshipAttribute("sliceofpieModel", "d_folderId", "folder", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Folder), "document", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.Document), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "d_projectId", "project", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Project), "document", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.Document), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "r_documentId", "document", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Document), "revision", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.Revision), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "f_folderId", "folder", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Folder), "folder1", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.Folder), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "f_projectId", "project", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Project), "folder", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.Folder), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "pu_projectId", "project", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.Project), "project_users", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.ProjectUser), true)]
[assembly: EdmRelationshipAttribute("sliceofpieModel", "pu_userId", "user", System.Data.Metadata.Edm.RelationshipMultiplicity.ZeroOrOne, typeof(SliceOfPie.User), "project_users", System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(SliceOfPie.ProjectUser), true)]

#endregion

namespace SliceOfPie
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class sliceofpieEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new sliceofpieEntities object using the connection string found in the 'sliceofpieEntities' section of the application configuration file.
        /// </summary>
        public sliceofpieEntities() : base("name=sliceofpieEntities", "sliceofpieEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new sliceofpieEntities object.
        /// </summary>
        public sliceofpieEntities(string connectionString) : base(connectionString, "sliceofpieEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new sliceofpieEntities object.
        /// </summary>
        public sliceofpieEntities(EntityConnection connection) : base(connection, "sliceofpieEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Document> Documents1
        {
            get
            {
                if ((_Documents1 == null))
                {
                    _Documents1 = base.CreateObjectSet<Document>("Documents1");
                }
                return _Documents1;
            }
        }
        private ObjectSet<Document> _Documents1;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Folder> Folders1
        {
            get
            {
                if ((_Folders1 == null))
                {
                    _Folders1 = base.CreateObjectSet<Folder>("Folders1");
                }
                return _Folders1;
            }
        }
        private ObjectSet<Folder> _Folders1;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Project> Projects1
        {
            get
            {
                if ((_Projects1 == null))
                {
                    _Projects1 = base.CreateObjectSet<Project>("Projects1");
                }
                return _Projects1;
            }
        }
        private ObjectSet<Project> _Projects1;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<ProjectUser> ProjectUsers1
        {
            get
            {
                if ((_ProjectUsers1 == null))
                {
                    _ProjectUsers1 = base.CreateObjectSet<ProjectUser>("ProjectUsers1");
                }
                return _ProjectUsers1;
            }
        }
        private ObjectSet<ProjectUser> _ProjectUsers1;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Revision> Revisions1
        {
            get
            {
                if ((_Revisions1 == null))
                {
                    _Revisions1 = base.CreateObjectSet<Revision>("Revisions1");
                }
                return _Revisions1;
            }
        }
        private ObjectSet<Revision> _Revisions1;
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<User> Users1
        {
            get
            {
                if ((_Users1 == null))
                {
                    _Users1 = base.CreateObjectSet<User>("Users1");
                }
                return _Users1;
            }
        }
        private ObjectSet<User> _Users1;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Documents1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToDocuments1(Document document)
        {
            base.AddObject("Documents1", document);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Folders1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToFolders1(Folder folder)
        {
            base.AddObject("Folders1", folder);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Projects1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToProjects1(Project project)
        {
            base.AddObject("Projects1", project);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the ProjectUsers1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToProjectUsers1(ProjectUser projectUser)
        {
            base.AddObject("ProjectUsers1", projectUser);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Revisions1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToRevisions1(Revision revision)
        {
            base.AddObject("Revisions1", revision);
        }
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Users1 EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToUsers1(User user)
        {
            base.AddObject("Users1", user);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="Document")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Document : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Document object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static Document CreateDocument(global::System.Int32 id)
        {
            Document document = new Document();
            document.Id = id;
            return document;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> FolderId
        {
            get
            {
                return _FolderId;
            }
            set
            {
                OnFolderIdChanging(value);
                ReportPropertyChanging("FolderId");
                _FolderId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FolderId");
                OnFolderIdChanged();
            }
        }
        private Nullable<global::System.Int32> _FolderId;
        partial void OnFolderIdChanging(Nullable<global::System.Int32> value);
        partial void OnFolderIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                OnProjectIdChanging(value);
                ReportPropertyChanging("ProjectId");
                _ProjectId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ProjectId");
                OnProjectIdChanged();
            }
        }
        private Nullable<global::System.Int32> _ProjectId;
        partial void OnProjectIdChanging(Nullable<global::System.Int32> value);
        partial void OnProjectIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                OnTitleChanging(value);
                ReportPropertyChanging("Title");
                _Title = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Title");
                OnTitleChanged();
            }
        }
        private global::System.String _Title;
        partial void OnTitleChanging(global::System.String value);
        partial void OnTitleChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "r_documentId", "revision")]
        public EntityCollection<Revision> Revisions
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Revision>("sliceofpieModel.r_documentId", "revision");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Revision>("sliceofpieModel.r_documentId", "revision", value);
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="Folder")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Folder : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Folder object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static Folder CreateFolder(global::System.Int32 id)
        {
            Folder folder = new Folder();
            folder.Id = id;
            return folder;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> FolderId
        {
            get
            {
                return _FolderId;
            }
            set
            {
                OnFolderIdChanging(value);
                ReportPropertyChanging("FolderId");
                _FolderId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("FolderId");
                OnFolderIdChanged();
            }
        }
        private Nullable<global::System.Int32> _FolderId;
        partial void OnFolderIdChanging(Nullable<global::System.Int32> value);
        partial void OnFolderIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                OnProjectIdChanging(value);
                ReportPropertyChanging("ProjectId");
                _ProjectId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ProjectId");
                OnProjectIdChanged();
            }
        }
        private Nullable<global::System.Int32> _ProjectId;
        partial void OnProjectIdChanging(Nullable<global::System.Int32> value);
        partial void OnProjectIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                OnTitleChanging(value);
                ReportPropertyChanging("Title");
                _Title = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Title");
                OnTitleChanged();
            }
        }
        private global::System.String _Title;
        partial void OnTitleChanging(global::System.String value);
        partial void OnTitleChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "d_folderId", "document")]
        public EntityCollection<Document> Documents
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Document>("sliceofpieModel.d_folderId", "document");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Document>("sliceofpieModel.d_folderId", "document", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "f_folderId", "folder1")]
        public EntityCollection<Folder> Folders
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Folder>("sliceofpieModel.f_folderId", "folder1");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Folder>("sliceofpieModel.f_folderId", "folder1", value);
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="Project")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Project : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Project object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static Project CreateProject(global::System.Int32 id)
        {
            Project project = new Project();
            project.Id = id;
            return project;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                OnTitleChanging(value);
                ReportPropertyChanging("Title");
                _Title = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Title");
                OnTitleChanged();
            }
        }
        private global::System.String _Title;
        partial void OnTitleChanging(global::System.String value);
        partial void OnTitleChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "d_projectId", "document")]
        public EntityCollection<Document> Documents
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Document>("sliceofpieModel.d_projectId", "document");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Document>("sliceofpieModel.d_projectId", "document", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "f_projectId", "folder")]
        public EntityCollection<Folder> Folders
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<Folder>("sliceofpieModel.f_projectId", "folder");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<Folder>("sliceofpieModel.f_projectId", "folder", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "pu_projectId", "project_users")]
        public EntityCollection<ProjectUser> project_users
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<ProjectUser>("sliceofpieModel.pu_projectId", "project_users");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<ProjectUser>("sliceofpieModel.pu_projectId", "project_users", value);
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="ProjectUser")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class ProjectUser : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new ProjectUser object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static ProjectUser CreateProjectUser(global::System.Int32 id)
        {
            ProjectUser projectUser = new ProjectUser();
            projectUser.Id = id;
            return projectUser;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ProjectId
        {
            get
            {
                return _ProjectId;
            }
            set
            {
                OnProjectIdChanging(value);
                ReportPropertyChanging("ProjectId");
                _ProjectId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ProjectId");
                OnProjectIdChanged();
            }
        }
        private Nullable<global::System.Int32> _ProjectId;
        partial void OnProjectIdChanging(Nullable<global::System.Int32> value);
        partial void OnProjectIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                OnUserIdChanging(value);
                ReportPropertyChanging("UserId");
                _UserId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("UserId");
                OnUserIdChanged();
            }
        }
        private Nullable<global::System.Int32> _UserId;
        partial void OnUserIdChanging(Nullable<global::System.Int32> value);
        partial void OnUserIdChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "pu_projectId", "project")]
        public Project project
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Project>("sliceofpieModel.pu_projectId", "project").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Project>("sliceofpieModel.pu_projectId", "project").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<Project> projectReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<Project>("sliceofpieModel.pu_projectId", "project");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<Project>("sliceofpieModel.pu_projectId", "project", value);
                }
            }
        }
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "pu_userId", "user")]
        public User user
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<User>("sliceofpieModel.pu_userId", "user").Value;
            }
            set
            {
                ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<User>("sliceofpieModel.pu_userId", "user").Value = value;
            }
        }
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [BrowsableAttribute(false)]
        [DataMemberAttribute()]
        public EntityReference<User> userReference
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedReference<User>("sliceofpieModel.pu_userId", "user");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedReference<User>("sliceofpieModel.pu_userId", "user", value);
                }
            }
        }

        #endregion
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="Revision")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Revision : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Revision object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static Revision CreateRevision(global::System.Int32 id)
        {
            Revision revision = new Revision();
            revision.Id = id;
            return revision;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                OnContentChanging(value);
                ReportPropertyChanging("Content");
                _Content = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Content");
                OnContentChanged();
            }
        }
        private global::System.String _Content;
        partial void OnContentChanging(global::System.String value);
        partial void OnContentChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ContentHash
        {
            get
            {
                return _ContentHash;
            }
            set
            {
                OnContentHashChanging(value);
                ReportPropertyChanging("ContentHash");
                _ContentHash = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ContentHash");
                OnContentHashChanged();
            }
        }
        private Nullable<global::System.Int32> _ContentHash;
        partial void OnContentHashChanging(Nullable<global::System.Int32> value);
        partial void OnContentHashChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> DocumentId
        {
            get
            {
                return _DocumentId;
            }
            set
            {
                OnDocumentIdChanging(value);
                ReportPropertyChanging("DocumentId");
                _DocumentId = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("DocumentId");
                OnDocumentIdChanged();
            }
        }
        private Nullable<global::System.Int32> _DocumentId;
        partial void OnDocumentIdChanging(Nullable<global::System.Int32> value);
        partial void OnDocumentIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();

        #endregion
    
    }
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="sliceofpieModel", Name="User")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class User : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new User object.
        /// </summary>
        /// <param name="id">Initial value of the Id property.</param>
        public static User CreateUser(global::System.Int32 id)
        {
            User user = new User();
            user.Id = id;
            return user;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Email
        {
            get
            {
                return _Email;
            }
            set
            {
                OnEmailChanging(value);
                ReportPropertyChanging("Email");
                _Email = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Email");
                OnEmailChanged();
            }
        }
        private global::System.String _Email;
        partial void OnEmailChanging(global::System.String value);
        partial void OnEmailChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    ReportPropertyChanging("Id");
                    _Id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }
        private global::System.Int32 _Id;
        partial void OnIdChanging(global::System.Int32 value);
        partial void OnIdChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String Password
        {
            get
            {
                return _Password;
            }
            set
            {
                OnPasswordChanging(value);
                ReportPropertyChanging("Password");
                _Password = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("Password");
                OnPasswordChanged();
            }
        }
        private global::System.String _Password;
        partial void OnPasswordChanging(global::System.String value);
        partial void OnPasswordChanged();

        #endregion
    
        #region Navigation Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [XmlIgnoreAttribute()]
        [SoapIgnoreAttribute()]
        [DataMemberAttribute()]
        [EdmRelationshipNavigationPropertyAttribute("sliceofpieModel", "pu_userId", "project_users")]
        public EntityCollection<ProjectUser> project_users
        {
            get
            {
                return ((IEntityWithRelationships)this).RelationshipManager.GetRelatedCollection<ProjectUser>("sliceofpieModel.pu_userId", "project_users");
            }
            set
            {
                if ((value != null))
                {
                    ((IEntityWithRelationships)this).RelationshipManager.InitializeRelatedCollection<ProjectUser>("sliceofpieModel.pu_userId", "project_users", value);
                }
            }
        }

        #endregion
    }

    #endregion
    
}
