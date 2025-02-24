using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITnetworkProjekt.Features.Insurances.Queries
{
    public class GetInsuredPersonSelectListQuery : IRequest<SelectList>
    {
        public int? SelectedId { get; }

        public GetInsuredPersonSelectListQuery(int? selectedId = null)
        {
            SelectedId = selectedId;
        }
    }
}