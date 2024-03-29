﻿using System.Drawing;
using System.Net;

namespace FuckResx;
public class Resource
{
    public Resource(Resources parent, string name, string link)
    {
        AssemblyName = parent.AssemblyName;
        Name = name;
        Link = link;
    }
    public Resource(string name, string link)
    {
        Name = name;
        Link = link;
    }
    public string? AssemblyName { get; set; }
    public string Name { get; private set; }
    public string Link { get; private set; }
    public string FullName => $"{AssemblyName ?? ""}.{Name}";
    private string Path => @$"C:\Users\{Environment.UserName}\AppData\Local\Temp\{FullName}";
    public bool IsExists => File.Exists(Path);
    public void Download() => new WebClient().DownloadFile(Link, Path);
    public T? GetContent<T>()
    {
        switch (typeof(T).Name)
        {
            case "Icon":
                return (T)(object)new Icon(Path);
            case "Image":
                return (T)(object)Image.FromFile(Path);
            case "Bitmap":
                return (T)(object)Bitmap.FromFile(Path);
            default: return default;
        }
    }
}
public class Resources
{
    public Resources(Type type, params Resource[] resources)
    {
        AssemblyName = type.FullName;
        this.resources = resources.ToList();
        for (int i = 0; i < this.resources.Count; i++)
            this.resources[i].AssemblyName = type.FullName;
    }
    public string? AssemblyName { get; private set; }
    private List<Resource> resources;
    public Resource? Get(string name) => resources.Find(z => z.Name.Equals(name));
    public T? GetContent<T>(string name) => Get(name).GetContent<T>();
    public void Add(string name, string link) => resources.Add(new Resource(this, name, link));
    public void Perfom() => resources.ForEach(z =>
    {
        if (!z.IsExists)
            z.Download();
    });
}