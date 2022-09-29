namespace UrlShortener.BLL.EntityServices;

/// <summary>
/// Інтерфейс сервісу з базовим функционалом для роботи з сутностями.
/// </summary>
/// <typeparam name="TEntity">Тип сутності.</typeparam>
/// <typeparam name="TKey">Тип первинного ключа сутності.</typeparam>
public interface IEntityService<TEntity, TKey>
where TEntity : class, new()
where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Отримати лист сутностей.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Проміс, що повертає кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    Task<(IEnumerable<TResult> Entities, int TotalCount)> GetAllAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    (IEnumerable<TResult> Entities, int TotalCount) GetAll<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей.
    /// </summary>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Проміс, що повертає кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetAllAsync(
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей.
    /// </summary>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спадання.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    (IEnumerable<TEntity> Entities, int TotalCount) GetAll(
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати сутність за айді.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="id">Айді сутності.</param>
    /// <returns>Проміс, що повертає знайдену сутність (або null, якщо не знайдено).</returns>
    Task<TResult?> GetByIdAsync<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Отримати сутність за айді.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="id">Айді сутності.</param>
    /// <returns>Знайдена сутність (або null, якщо не знайдено).</returns>
    TResult? GetById<TResult>(TKey id, Expression<Func<TEntity, TResult>> selector);

    /// <summary>
    /// Отримати сутність за айді.
    /// </summary>
    /// <param name="id">Айді сутності.</param>
    /// <returns>Проміс, що повертає знайдену сутність (або null, якщо не знайдено).</returns>
    Task<TEntity?> GetByIdAsync(TKey id);

    /// <summary>
    /// Отримати сутність за айді.
    /// </summary>
    /// <param name="id">Айді сутності.</param>
    /// <returns>Знайдена сутність (або null, якщо не знайдено).</returns>
    TEntity? GetById(TKey id);

    /// <summary>
    /// Записує нову сутність до бази даних. 
    /// Якщо виникає конфлікт айді - для нової сутності буде призначено нове айді.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно додати.</param>
    /// <returns>Проміс, що повертає додану сутність (можа отримати айді створеної сутності).</returns>
    Task<TEntity> CreateAsync(TEntity entity);

    /// <summary>
    /// Записує нову сутність до бази даних. 
    /// Якщо виникає конфлікт айді - для нової сутності буде призначено нове айді.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно додати.</param>
    /// <returns>Додана сутність (можа отримати айді створеної сутності).</returns>
    TEntity Create(TEntity entity);

    /// <summary>
    /// Оновлює існуючу сутність в базі даних (перезапис всіх даних). 
    /// Якщо сутності не існувало - створюється нова з новим айді.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно оновити.</param>
    /// <returns>Проміс, що повертає оновлену сутність (якщо було створено, то можна отримати айді створеної сутності).</returns>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Оновлює існуючу сутність в базі даних (перезапис всіх даних). 
    /// Якщо сутності не існувало - створюється нова з новим айді.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно оновити.</param>
    /// <returns>Оновлена сутність (якщо було створено, то можна отримати айді створеної сутності).</returns>
    TEntity Update(TEntity entity);

    /// <summary>
    /// Видаляє всі дані з таблиці цієї сутності.
    /// </summary>
    /// <returns>Проміс операції.</returns>
    Task RemoveAllAsync();

    /// <summary>
    /// Видаляє всі дані з таблиці цієї сутності.
    /// </summary>
    void RemoveAll();

    /// <summary>
    /// Видаляє сутність з бази даних.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно видалити.</param>
    /// <returns>Проміс, що повертає булєве значення стосовно існування сутності до видалення.</returns>
    Task<bool> RemoveAsync(TEntity entity);

    /// <summary>
    /// Видаляє сутність з бази даних.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно видалити.</param>
    /// <returns>Булєве значення стосовно існування сутності до видалення.</returns>
    bool Remove(TEntity entity);

    /// <summary>
    /// Знаходить та видаляє сутність з бази даних за її айді.
    /// </summary>
    /// <param name="id">Айді сутності для видалення.</param>
    /// <returns>Проміс, що повертає булєве значення стосовно існування сутності до видалення.</returns>
    Task<bool> RemoveByIdAsync(TKey id);

    /// <summary>
    /// Знаходить та видаляє сутність з бази даних за її айді.
    /// </summary>
    /// <param name="id">Айді сутності для видалення.</param>
    /// <returns>Булєве значення стосовно існування сутності до видалення.</returns>
    bool RemoveById(TKey id);
}
