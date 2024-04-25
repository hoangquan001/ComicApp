Project asp.net core about the comic app. This app provides restful API for a list of comic
Technology: ASP.net Core, Entity Framework, MS SQL, Authentication, Authorization, Restful API, JWT, 


# Comics API

Vietnamese Comics API 

## Usage

### **Comic**
```ts
param:
    page: number; // option
    step: number; // option
    sort: SortType; // option    
    //[Chapter,LastUpdate, TopFollow, TopComment, NewComic, TopDay, TopWeek, TopMonth,TopAll]
    status: number; // option [-1: all, 0: Ongoing, 1:COmplete]
    genre: number; // option
result:
{
    
}
path: `/comics?${param}`;
```

### **Genres**

```ts
path: '/genres';
```


### **Search**

```ts
query: string;
page: number; // option

path: `/search?q=${query}&page=${page}`;
```

### **Search Suggest**

```ts
query: string;

path: `/search-suggest?q=${query}`;
```

### **Recommend Comics**

```ts
path: '/recommend-comics';
```

### **New Comics**

```ts
page: number; // option

status: 'all' | 'completed' | 'ongoing'; // option

path: `/new-comics?page=${page}&status=${status}`;
```

### **Recent Update Comics**

```ts
page: number; // option
status: 'all' | 'completed' | 'ongoing'; // option

path: `/recent-update-comics?page=${page}&status=${status}`;
```


### **Comic Detail**

```ts
comic_id: string;

path: `/comics/${comic_id}`;
```

### **Comic Chapters**

```ts
comic_id: string;

path: `/comics/${comic_id}/chapters`;
```

### **Single Chapter**

```ts
comic_id: string;
chapter_id: number;

path: `/comics/${comic_id}/chapters/${chapter_id}`;
```

### **Top**

```ts
page: number; // option
status: 'all' | 'completed' | 'ongoing'; // option

// Paths
All: `/top?page=${page}&status=${status}`;
Daily: `/top/daily?page=${page}&status=${status}`;
Weekly: `/top/weekly?page=${page}&status=${status}`;
Monthly: `/top/monthly?page=${page}&status=${status}`;
Chapter: `/top/chapter?page=${page}&status=${status}`;
Follow: `/top/follow?page=${page}&status=${status}`;
Comment: `/top/comment?page=${page}&status=${status}`;
```
