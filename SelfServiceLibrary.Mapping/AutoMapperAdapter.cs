﻿
using SelfServiceLibrary.Service.Interfaces;

namespace SelfServiceLibrary.Mapping
{
    /// <summary>
    /// Adapts independent IMapper interface onto AutoMapper implementation
    /// </summary>
    public class AutoMapperAdapter : IMapper
    {
        private readonly AutoMapper.IMapper _autoMapper;

        public AutoMapperAdapter(AutoMapper.IMapper autoMapper) => 
            _autoMapper = autoMapper;

        public TDestination Map<TDestination>(object source) =>
            _autoMapper.Map<TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source) =>
            _autoMapper.Map<TSource, TDestination>(source);

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination) =>
            _autoMapper.Map(source, destination);
    }
}
