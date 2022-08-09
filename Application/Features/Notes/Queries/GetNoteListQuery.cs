﻿using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedModels.Enums;
using SharedModels.Responses.Notes;

namespace Application.Features.Notes.Queries;
public class GetNoteListQuery : IRequest<List<NoteResponse>>
{
    public string Filter { get; set; }
}

public class GetNoteListQueryHandler : IRequestHandler<GetNoteListQuery, List<NoteResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    public GetNoteListQueryHandler(IAppDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<List<NoteResponse>> Handle(GetNoteListQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Notes
            .Include(n => n.Permissions)
            .Where(n => n.CreatedBy == _currentUserService.UserId ||
                        n.Permissions.Any(p => p.Email == _currentUserService.Email && (p.Scope == (int)PermissionScope.Read || p.Scope == (int)PermissionScope.ReadAndWrite)));

        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(n => n.Title.ToLower().Contains(request.Filter.ToLower()) || n.Content.ToLower().Contains(request.Filter.ToLower()));

        var noteResponseList = await query
        .Select(n => new NoteResponse()
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            Owned = n.CreatedBy == _currentUserService.UserId
        })
        .ToListAsync(cancellationToken);

        return noteResponseList;
    }
}