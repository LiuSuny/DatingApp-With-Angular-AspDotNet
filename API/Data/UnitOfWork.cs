using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

         public UnitOfWork(DataContext context, IMapper mapper)
         {
            _mapper = mapper;
            _context = context;      
        }         
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);

         //method response for efcore changes saving changes track
        public async Task<bool> Complete()
        {
           return await _context.SaveChangesAsync() > 0 ;
        }

        public bool HasChanges()
        {
            //check if it has changes
            return _context.ChangeTracker.HasChanges();
        }
    }
}