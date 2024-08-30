# OdinCola - Middlewares
OdinCola - Middlewares netcore8.0

|序号| 中间件名称 | 中文说明    | readme                                      |
|:--|:------|:--------|:--------------------------------------------|
|1 | ExceptionMiddleware | 异常处理中间件 | [ExceptionMiddleware](#ExceptionMiddleware) |

### 配置说明

#### ExceptionMiddleware

> 需要引用 Cola.Models.Core 以及 Cola.Log 并配置 Cola.Log

使用示例:

```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// 在此处加入异常处理中间件 尽量在外层的中间件 可以捕获到更多的错误
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
```
