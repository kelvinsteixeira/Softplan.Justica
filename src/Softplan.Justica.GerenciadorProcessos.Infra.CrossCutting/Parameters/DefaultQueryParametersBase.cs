namespace Softplan.Justica.GerenciadorProcessos.Infra.CrossCutting.Parameters
{
    public abstract class DefaultQueryParametersBase
    {
        private const int MaxPageSize = 50;
        private int pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                this.pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
    }
}