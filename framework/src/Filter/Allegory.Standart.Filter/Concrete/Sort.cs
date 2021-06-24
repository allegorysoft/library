using Allegory.Standart.Filter.Enums;
using Allegory.Standart.Filter.Properties;

namespace Allegory.Standart.Filter.Concrete
{
    public sealed class Sort
    {
        #region Constructors
        public Sort() { }
        public Sort(string column, OrderDirection orderDirection = OrderDirection.Asc)
        {
            Column = column;
            OrderDirection = orderDirection;
        }
        #endregion

        #region Properties
        public string Column { get; set; }
        public OrderDirection OrderDirection { get; set; }
        #endregion

        #region Methods
        public void ValidateColumn()
        {
            if (string.IsNullOrEmpty(Column))
                throw new FilterException(Resource.ColumnNullError);
        }
        #endregion

        public override string ToString()
        {
            ValidateColumn();
            string[] column = Column.Replace("[", "[[").Replace("]", "]]").Split('.');
            string columnName = column.Length > 1
                ? "[" + column[0] + "].[" + column[1] + "]"
                : "[" + Column.Replace("[", "[[").Replace("]", "]]") + "]";

            return string.Format($"{columnName} {OrderDirection.ToString()}");
        }
    }
}
