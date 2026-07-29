namespace FooBar.Domain.Invoices.Model.Entity
{
    // ⚠️ IMPORTANTE: Los enums en C# se inicializan a 0 por defecto.
    // Canceled = 0 significa que una factura sin estado asignado quedará
    // "cancelada" por defecto, lo cual es un bug semántico.
    //
    // Soluciones recomendadas:
    // 1. Reordenar: Active primero (= 0), Canceled después (= 1)
    // 2. Inicializar en el constructor de Invoice: Estado = InvoiceState.Active
    // 3. Usar [Required] o propiedad 'required' en C# 11+
    //
    // Ver: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
    public enum InvoiceState
    {
        Canceled,
        Active
    }
}
