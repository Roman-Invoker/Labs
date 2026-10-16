public class InMemoryRepositoryTests
{
    private static InMemoryRepository<Course> CreateRepo() => new();

    private static Course MakeCourse(int id, string title = "Test") =>
        new() { Id = id, Title = title, Credits = 3, TeacherId = 100 };

    // ---------- Add ----------
    [Fact]
    public void Add_NewItem_AppearsInGetAll()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(1));
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Add_DuplicateId_Throws()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(1));
        Assert.Throws<InvalidOperationException>(() => repo.Add(MakeCourse(1)));
    }

    [Fact]
    public void Add_Null_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentNullException>(() => repo.Add(null!));
    }

    // ---------- GetById ----------
    [Fact]
    public void GetById_Existing_ReturnsItem()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(5, "Алгоритми"));
        var result = repo.GetById(5);
        Assert.NotNull(result);
        Assert.Equal("Алгоритми", result!.Title);
    }

    [Fact]
    public void GetById_NonExisting_ReturnsNull()
    {
        var repo = CreateRepo();
        Assert.Null(repo.GetById(999));
    }

    // ---------- GetAll ----------
    [Fact]
    public void GetAll_EmptyRepo_ReturnsEmpty()
    {
        Assert.Empty(CreateRepo().GetAll());
    }

    [Fact]
    public void GetAll_WithItems_ReturnsAll()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(1));
        repo.Add(MakeCourse(2));
        repo.Add(MakeCourse(3));
        Assert.Equal(3, repo.GetAll().Count());
    }

    // ---------- Update ----------
    [Fact]
    public void Update_Existing_ChangesItem()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(1, "Old"));
        repo.Update(new Course { Id = 1, Title = "New", Credits = 6, TeacherId = 200 });

        var updated = repo.GetById(1)!;
        Assert.Equal("New", updated.Title);
        Assert.Equal(6, updated.Credits);
        Assert.Equal(200, updated.TeacherId);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Update_NonExisting_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.Update(MakeCourse(42)));
    }

    // ---------- Delete ----------
    [Fact]
    public void Delete_Existing_RemovesItem()
    {
        var repo = CreateRepo();
        repo.Add(MakeCourse(1));
        repo.Add(MakeCourse(2));

        repo.Delete(1);

        Assert.Null(repo.GetById(1));
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Delete_NonExisting_Throws()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.Delete(42));
    }
}
