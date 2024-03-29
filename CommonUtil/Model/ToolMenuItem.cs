﻿using AutoMapper;

namespace CommonUtil.Model;

public readonly record struct ToolMenuItem {
    public string Id { get; }
    public string Name { get; }
    public string ImagePath { get; }
    public Type ClassType { get; }

    public ToolMenuItem(string id, string name, string imagePath, Type classType) {
        Id = id;
        Name = name;
        ImagePath = imagePath;
        ClassType = classType;
    }
}

public class ToolMenuItemDO : DependencyObject {
    public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ToolMenuItemDO), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(ToolMenuItemDO), new PropertyMetadata());

    public string Id { get; }
    public Type ViewType { get; }
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name {
        get { return (string)GetValue(NameProperty); }
        set { SetValue(NameProperty, value); }
    }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public ToolMenuItemDO(string id, Type viewType) {
        Id = id;
        ViewType = viewType;
    }
}

public class ToolMenuItemProfile : Profile {
    public ToolMenuItemProfile() {
        CreateMap<ToolMenuItem, ToolMenuItem>();
        CreateMap<ToolMenuItem, ToolMenuItemDO>()
            .ConstructUsing(src => new ToolMenuItemDO(src.Id, src.ClassType))
            .ForMember(dest => dest.Icon, cfg => cfg.MapFrom(src => src.ImagePath));
        CreateMap<ToolMenuItemDO, ToolMenuItemDO>();
        CreateMap<ToolMenuItemDO, ToolMenuItem>()
            .ForMember(dest => dest.ClassType, cfg => cfg.MapFrom(src => src.ViewType))
            .ForMember(dest => dest.ImagePath, cfg => cfg.MapFrom(src => src.Icon));
    }
}