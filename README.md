# Volume Render

> Try using a more appropriate volumetric rendering mode to render cells or other z-stack images.

## Processing

```mermaid

graph LR
    load[load] --> input[preview / adjust]
    input --> filter[filter]
    filter --> interpolation[interpolation]
    interpolation --> resize[resize 300 x 300 x 300]
    resize --> render[render]
```

## Todos

- [x] Preview and adjust images data
- [x] Base Volume Render
- [ ] 3D Filter
- [ ] Z-data interpolation

## Reference & Thanks

- [helix-toolkits](https://github.com/helix-toolkit/helix-toolkit)
- [handycontrol](https://github.com/HandyOrg/HandyControl)