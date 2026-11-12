using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BlogWithEf.Models;

namespace BlogWithEf.Controllers;

public class PostsController : Controller
{
    private readonly BlogDbContext _context; // initializes a read only BlogDbContext named _context, which is used to interact with the database

    public PostsController(BlogDbContext context) // constructor for the PostsController class that takes a BlogDbContext parameter named context
    {
        _context = context;
    }

    public async Task<IActionResult> Index() // takes all the posts from the database and orders them by the CreatedUtc property in descending order so that the most recent posts appear first, the result is then passed to the Index view for rendering
    {
        var posts = await _context.Posts.OrderByDescending(post => post.CreatedUtc).ToListAsync();
        return View(posts);
    }

    [HttpGet]
    public IActionResult Create() // returns the Create view, which contains a form for creating a new post
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Post post) // handles the form submission for creating a new post. tt first checks if the model state is valid, and if not, it returns the Create view with the current post data. ff the model state is valid, it adds the new post to the database context, saves the changes asynchronously, and then redirects to the Index action to display the list of posts
    {
        if (!ModelState.IsValid)
        {
            return View(post);
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id) // retrieves a post from the database based on the provided id. if the post is found, it returns the Edit view with the post data for editing. if the post is not found, it returns a NotFound result
    {
        var post = await _context.Posts.FindAsync(id);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Post post) // handles the form submission for editing an existing post. it first checks if the provided id matches the id of the post being edited. if they don't match, it returns a NotFound result. then, it checks if the model state is valid, and if not, it returns the Edit view with the current post data. if the model state is valid, it retrieves the existing post from the database based on the provided id. if the existing post is found, it updates its Title and Body properties with the new values from the submitted post. finally, it saves the changes asynchronously and redirects to the Index action to display the updated list of posts
    {
        if (id != post.ID)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(post);
        }

        var existingPost = await _context.Posts.FindAsync(id);

        if (existingPost is null)
        {
            return NotFound();
        }

        existingPost.Title = post.Title;
        existingPost.Body = post.Body;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id) // handles the deletion of a post based on the provided id. it retrieves the post from the database using the FindAsync method. if the post is found, it removes it from the database context and saves the changes asynchronously. finally, it redirects to the Index action to display the updated list of posts. if the post is not found, it returns a NotFound result
    {
        var post = await _context.Posts.FindAsync(id);

        if (post is null)
        {
            return NotFound();
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
