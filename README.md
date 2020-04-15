# ToolsToLive.Hierarchy
Tools to convert a flat list to a hierarchy based on ParentId properties and work with hierarchy collection (child or parent search, element selecting etc.).

## How to use
An example with dotnet core. For dotnet framework you can use it the same way but most likely you will have to set up dependency injetion manually.

### Prerequisites
 - Reference to ToolsToLive.JsonSourceLocalizer (you can found it in nuget);
 - A model that participates in the hierarchy must implement the "IHierarchyItem<T, TId, TParentId>" interface.
 - Set up dependency injection by calling "serivces.AddHierarchyToolsForXXX(opt => { ... } )" in your Startup.cs file, where XXX - IntId or StringId depending on your model.
 - Inject "IHierarchyTools<T, TId, TParentId>" wherever you need localization.

#### Setup
  ```csharp
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHierarchyToolsForIntId<SpecializationModel>(opt =>
            {
                opt.SetParents = false;
            });

            //// or you can pass configuration section, instead of setting up options here. But in most cases if you change this parameters you need to change code anyway.
            // var optionsSection = Configuration.GetSection("SomeSectionInAppSettingsFile");
            // services.AddHierarchyToolsForIntId(optionsSection);

            // other code
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // other code
        }
    }
```
#### Usage
 ```csharp
    // if your model uses string as Id -- just implement IHierarchyItem<MyModel, string, string> interface
    public class MyModel : IHierarchyItem<MyModel, int, int?>
    {
        /// implementation and whatever you need
    }

    public class MyService : IMyService
    {
        private readonly IHierarchyTools<MyModel, int, int?> _hierarchyTools;

        public MyService(IHierarchyTools<MyModel, int, int?> hierarchyTools)
        {
            _hierarchyTools = hierarchyTools;
        }

        private async Task<List<MyModel>> GetHierarchyList()
        {
            List<MyModel> plainList = await _dbContext.MyModels.AsNoTracking()
                .Select(x => new MyModel
                {
                    Id = x.MyId,
                    ParentId = x.ParentId,
                    Name = x.Name,
                    // anything else
                })
                .ToListAsync();

            List<MyModel> hierarchyList = _hierarchyTools.ToHierarhyList(plainList);
            return hierarchyList; // this list contains elements on the top level (elements with ParentId == null). And each element contain nested elements in "Childs" list (or empty collection if there is no children for this element).
        }
    }
```