namespace EasyShop.MVC.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET: /Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        ViewBag.CategoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

        return View(categories);
    }

    // GET: /Categories/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        ViewBag.ParentCategoryName = await GetParentNameAsync(category.ParentCategoryId);
        return View(category);
    }

    // GET: /Categories/Create
    public async Task<IActionResult> Create()
    {
        await PopulateParentDropdownAsync();
        return View(new CategoryFormViewModel());
    }

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateParentDropdownAsync(model.ParentCategoryId);
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name.Trim(),
            Description = model.Description,
            ParentCategoryId = model.ParentCategoryId,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryService.AddCategoryAsync(category);

        TempData["Success"] = $"Category \"{category.Name}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        await PopulateParentDropdownAsync(category.ParentCategoryId, category.Id);

        return View(new CategoryFormViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive
        });
    }

    // POST: /Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (await IsInvalidParentAsync(id, model.ParentCategoryId))
        {
            ModelState.AddModelError(nameof(model.ParentCategoryId), "A category can't be its own parent or one of its own sub-categories.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateParentDropdownAsync(model.ParentCategoryId, id);
            return View(model);
        }

        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        category.Name = model.Name.Trim();
        category.Description = model.Description;
        category.ParentCategoryId = model.ParentCategoryId;
        category.IsActive = model.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryService.UpdateCategory(category);

        TempData["Success"] = $"Category \"{category.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categories/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        ViewBag.ParentCategoryName = await GetParentNameAsync(category.ParentCategoryId);
        return View(category);
    }

    // POST: /Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        if (categories.Any(c => c.ParentCategoryId == id))
        {
            TempData["Error"] = $"Category \"{category.Name}\" can't be deleted while it still has sub-categories.";
            return RedirectToAction(nameof(Index));
        }

        _categoryService.DeleteCategory(category);

        TempData["Success"] = $"Category \"{category.Name}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> GetParentNameAsync(int? parentId)
    {
        if (parentId == null)
        {
            return null;
        }

        return (await _categoryService.GetCategoryByIdAsync(parentId.Value))?.Name;
    }

    private async Task PopulateParentDropdownAsync(int? selectedId = null, int? excludeId = null)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        var excluded = new HashSet<int>();
        if (excludeId != null)
        {
            excluded.Add(excludeId.Value);
            excluded.UnionWith(GetDescendantIds(categories, excludeId.Value));
        }

        var options = categories
            .Where(c => !excluded.Contains(c.Id))
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });

        ViewBag.ParentCategories = new SelectList(options, "Value", "Text", selectedId);
    }

    private async Task<bool> IsInvalidParentAsync(int categoryId, int? parentId)
    {
        if (parentId == null)
        {
            return false;
        }

        if (parentId.Value == categoryId)
        {
            return true;
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        return GetDescendantIds(categories, categoryId).Contains(parentId.Value);
    }

    private static HashSet<int> GetDescendantIds(IEnumerable<Category> categories, int rootId)
    {
        var lookup = categories
            .Where(c => c.ParentCategoryId != null)
            .GroupBy(c => c.ParentCategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Id).ToList());

        var descendants = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(rootId);

        while (pending.Count > 0)
        {
            if (!lookup.TryGetValue(pending.Pop(), out var children))
            {
                continue;
            }

            foreach (var child in children)
            {
                if (descendants.Add(child))
                {
                    pending.Push(child);
                }
            }
        }

        return descendants;
    }
}
