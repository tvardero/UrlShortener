using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace UrlShortener.BLL.EntityServices;

/// <summary>
/// Інтерфейс сервісу з базовим функционалом для роботи з сутностями за допомогою Entity Framework.
/// </summary>
/// <typeparam name="TEntity">Тип сутності.</typeparam>
/// <typeparam name="TKey">Тип первинного ключа сутності.</typeparam>
public interface IEFEntityService<TEntity, TKey> : IEntityService<TEntity, TKey>
where TEntity : class, new()
where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Отримати лист сутностей з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Проміс, що повертає кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    Task<(IEnumerable<TResult> Entities, int TotalCount)> GetAllWithTrackingAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    (IEnumerable<TResult> Entities, int TotalCount) GetAllWithTracking<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спаданням.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Проміс, що повертає кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    Task<(IEnumerable<TEntity> Entities, int TotalCount)> GetAllWithTrackingAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати лист сутностей з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <param name="skip">Пагінація.</param>
    /// <param name="take">Пагінація.</param>
    /// <param name="orderBy">Сортування.</param>
    /// <param name="orderByDescending">Сортування за зростанням чи спадання.</param>
    /// <param name="filters">Лист з фільтрами.</param>
    /// <returns>Кортеж з двома параметрами: лист отриманних сутностей та загальна кількість знайдених сутностей (без пагинації).</returns>
    (IEnumerable<TEntity> Entities, int TotalCount) GetAllWithTracking(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll,
        int skip = 0,
        int take = 10,
        Expression<Func<TEntity, object?>>? orderBy = null,
        bool orderByDescending = false,
        Expression<Func<TEntity, bool>>[]? filters = null);

    /// <summary>
    /// Отримати сутність за айді з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="id">Айді сутності.</param>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <returns>Проміс, що повертає знайдену сутність (або null, якщо не знайдено).</returns>
    Task<TResult?> GetByIdWithTrackingAsync<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll);

    /// <summary>
    /// Отримати сутність за айді з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <typeparam name="TResult">Тип обранних даних за допомогою SELECT.</typeparam>
    /// <param name="id">Айді сутності.</param>
    /// <param name="selector">Вибірка потрібних параметрів.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <returns>Знайдена сутність (або null, якщо не знайдено).</returns>
    TResult? GetByIdWithTracking<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll);

    /// <summary>
    /// Отримати сутність за айді з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <param name="id">Айді сутності.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <returns>Проміс, що повертає знайдену сутність (або null, якщо не знайдено).</returns>
    Task<TEntity?> GetByIdWithTrackingAsync(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll);

    /// <summary>
    /// Отримати сутність за айді з вибором режиму EF відслідкування об'єктів.
    /// </summary>
    /// <param name="id">Айді сутності.</param>
    /// <param name="trackingBehavior">Режим роботи EF відслідкування об'єктів.</param>
    /// <returns>Знайдена сутність (або null, якщо не знайдено).</returns>
    TEntity? GetByIdWithTracking(
        TKey id,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>[]? includes = null,
        QueryTrackingBehavior trackingBehavior = QueryTrackingBehavior.TrackAll);

    /// <summary>
    /// Зберігти (застосувати) всі зроблені зміни до бази даних.
    /// </summary>
    /// <returns>Проміс, що повертає кількість рядків, що було створено, змінено або видалено.</returns>
    Task<int> SaveAsync();

    /// <summary>
    /// Зберігти (застосувати) всі зроблені зміни до бази даних.
    /// </summary>
    /// <returns>Кількість рядків, що було створено, змінено або видалено.</returns>
    int Save();

    /// <summary>
    /// Записує нову сутність до контексту, але не зберігає її в базі даних одразу. 
    /// Результат створення має зміст лише після виклику <see cref="SaveAsync" /> або <see cref="Save" />. 
    /// Якщо виникає конфлікт айді - для нової сутності буде призначено нове айді.
    /// </summary>
    /// <remarks>
    /// Щоб отримати саму сутність або її айді (після зберігання в базі даних), зверніться до <see cref="EntityEntry.Entity" />.
    /// </remarks>
    /// <param name="entity">Сутність, яку треба додати.</param>
    /// <returns>Об'єкт, що відслідковує додану сутність.</returns>
    EntityEntry<TEntity> CreateWithNoSave(TEntity entity);

    /// <summary>
    /// Оновлює існуючу сутність в контексті (перезапис всіх даних), але не зберігає зміни в базі даних одразу. 
    /// Результат оновлення має зміст лише після виклику <see cref="SaveAsync" /> або <see cref="Save" />. 
    /// Якщо сутності не існувало - створюється нова з новим айді.
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно оновити.</param>
    /// <returns>Об'єкт, що відслідковує оновлену сутність.</returns>
    EntityEntry<TEntity> UpdateWithNoSave(TEntity entity);

    /// <summary>
    /// Видаляє сутність з контексту, але не видаляє з бази даних одразу.
    /// Результат видалення має зміст лише після виклику <see cref="SaveAsync" /> або <see cref="Save" />. 
    /// </summary>
    /// <param name="entity">Сутність, яку потрібно видалити.</param>
    /// <returns>Об'єкт, що відслідковує видалену сутність.</returns>
    EntityEntry<TEntity> RemoveWithNoSave(TEntity entity);

    /// <summary>
    /// Знаходить та видаляє сутність з контексту за її айді, але не видаляє з бази даних одразу.
    /// </summary>
    /// <param name="id">Айді сутності для видалення.</param>
    /// <returns>Проміс, що повертає об'єкт відслідковування видаленої сутності.</returns>
    Task<EntityEntry<TEntity>?> RemoveByIdWithNoSaveAsync(TKey id);

    /// <summary>
    /// Знаходить та видаляє сутність з контексту за її айді, але не видаляє з бази даних одразу.
    /// </summary>
    /// <param name="id">Айді сутності для видалення.</param>
    /// <returns>Об'єкт, що відслідковує видалену сутність.</returns>
    EntityEntry<TEntity>? RemoveByIdWithNoSave(TKey id);
}
