using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Ks.Core.Domain.Directory;
using System.IO;
using System.Linq;
using Ks.Core;
using Ks.Core.Domain;
using Ks.Core.Domain.Batchs;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Reports;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Ks.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly IKsSystemService _ksSystemService;
        private readonly KsSystemInformationSettings _ksSystemInformationSettings;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly SignatureSettings _signatureSettings;

        #endregion

        #region Ctor

        public ExportManager(IKsSystemService ksSystemService,
            KsSystemInformationSettings ksSystemInformationSettings,
            IWebHelper webHelper, IDateTimeHelper dateTimeHelper, SignatureSettings signatureSettings)
        {
            _ksSystemService = ksSystemService;
            _ksSystemInformationSettings = ksSystemInformationSettings;
            _webHelper = webHelper;
            _dateTimeHelper = dateTimeHelper;
            _signatureSettings = signatureSettings;
        }

        #endregion

        private const String IMAGE =
            "iVBORw0KGgoAAAANSUhEUgAAAVQAAABGCAYAAACT+2yRAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAASRBJREFUeNrsXQd8VFXePdNLeq+kk0oChBZqQDrSqyBNsQB2xXVdRUUBO6ACglQpIh0ivQRIQgJJSEhCGum99zJ95rt3MgkTSAFXd12/d34+mcy8esu551/ufQADBgwYMGDAgAEDBgwYMGDAgAEDBgwYMGDAgAEDBgwYMGDA4H8DrP/0BYcPH+5haWVlr1ar2a3fKRWKqrNnzyYx1cGAAQOGUDuAjZ2dcOTIkf2lEskI8mcAW60ZU2rCs6jns8BVAwmGXO3VezUqtftz1Rq4ViubCc1eIn/e5fF4USnJyZEpKSmNTDUxYMDg/x2h8vh87qzZs3tLm5qfk3FZ82Ks+JaV5oQ4TQSAqyvZgzBpRT5g6wbwxUB+CmDVAy+49MbOqGOEVfmATAPUKYAqJUZVyRpMm9Sn+QaiHddDQ2+VlZXJmSpjwIDB35pQ2Ww2JdIpEqXivTQL3qBMRyFgxgM4hBwVMqwb/SLe8x0GDpsD1pnvMcnUGgGG5vgi8jAOPf0G/E1s0CtkI1BfRgjWGT/0GY9mlRLv3b0I5OWBVajExDJZuglf9M3p06cONTc3NzFVx4ABg78auP/uCabPmBGogmbbTTPWgGJ3E8DTB2XjVsBMIEbQjYOIK8vEUCtHcM9+33JAdRFgagMl4VoYWsGCJ8I7964hoIcvEjMbcXzwbNiJjFAnl2HngGl4gXcemr5WOJeT7IWc6h3jnp7wnhmb99ExApVKpWCqkAEDBv/zhDp+4kSBUCBYU2TMeS/G25CY9WxAZIrVroGwubqbmOxFWNfvaQQZW6BBQSx1NrlUUTohUVMYcLhoUsnxUe+xSG+oghH5bbGzN54tSME4u54wOvQxIBCgj4079vefgiqZFG/m3AVcRLhkL/IwvN/0y6SpUxbw2JwXTxw/XspUIwMGDP4KYP+eg0aPHu3DEYtun/YU/SNm1mBcf+4jfDXyOXI2HkyI4kQtMd0rcpDdVAtLrhA1cik2uPXFJ4Nn4eTwBXAUGkBMSPUN7yDt+fqbWGMU9atqlKiUNbWY/s0NuFuRB0OuAGfKc/F18CJEz/0E5cu/Q+OkIPzmbzy5nqPJmTZ9+iimGhkwYPA/qVBHDh/xNMvGbN85XwNzGBPuHL8C86JP4ylzB8DYDEn15XjGvT9+LUxGs0aF+821+DQ5FLfGvoQBlk7gXNoBNFXB2tgains38P3tE+QuBLAXGwHkHBwWG1dnfwg1OTaxroKQsQSLHb0RVV2Ed4/uA6xdsM5rKD4oy8EVM65wYHx96OTJk987c+bMV0x1MmDA4L+JJwpKDR0YtFLl67z+lp/YBGIiblVK5Dy3EeZ8IYz5IhQ31+Hz1AjMcfSFQq2CUqPBhPgLQEIUIBOiR6MGNnI1TJuVGO7mAxGXh7jiXGSpmlHDYyHTmIuebh6Y7R+Iz0vS8amTP2xFBlqF+15BCkCIGS59sLbnYHwYexqbBs/Fm7G/wTwiWxmUXbfh3OUL7zFVyoABg788oQ7sE7hE2dfj27g+Zha/TnsFBwjBnUm6ChAihb09oFTiJ+/xqCAm+wdh++Fs6Qvj1Dz0rJRikl8gevfvBwcHB1hZWYHLfVQY19fXIzs7G8lJyYhLScLxxiLkBTgDJnx813ss3shN1BLqsVmrMTstAh/a9sRnvUeDdWwtUJoN18QmVWC54nR5deWs8PBwpmYZMGDw10QfX785w5+fX4ovl2iwbbkmsiJXsy7puuZKcYYGp7/UrLs9VaPRNGoKG+s1OLheY7vyGc3n8+ZoEvbv1Sg0vw+FBYWarVu3ajxXLNAEH9pIrpmnIea/ZkHkMc0/4i9qmhUy7X448qmmUtKgwffLNHhzgWx00LCNTI0xYMDgvwFOdzv4uHtOsh7Wd8sNT4EdTHiApB79evTCO6lhYEOD+KeWIU3igf7hJ3Hh1Em8cjsbn8gVmGBjDvuxE8G1tf9dN2ZsbIwBAwZg3qBgaC5HYXXIL1h1/yySBFwc6DcFpwpTYcDlY61fMF69exH3ilPIQXyOBUTeLhxxbUFx0R2mehkwYPCfRJdRfjcnFyevwYFvh7rwHGBKzHS1RhtAWhl3FglPLcUUWw+wjn6GiLwMDDkXg88zy7Ccp4KDWgGNjR04Lm7/9g1aW1riZVsz7KxowjPJCng0c+B6/gfkNtfD28QKB/OScCg2hAwNhOwFLMT6iEwFfq7v9g/oO5ipXgZ/VURERAwhBtZ71tbWRkxp/D8gVDs7O96goYNfO+XIGQ0rPqDS6DQtIdbidMKtGhwrvg/k5kDwy3lsl6swhKMC3UvNYoHXwwUsU7N/+wZVJUVoKimEOyHLL6HG0vN3gaRk+BiaIbm2FK9e3dWS40pBLy5m45qH0N3Br+cnvXx9+UwVM/ir4fz58y8GDux3Zt3eb75YtGb5BxwOh2mnfxN0mjbFV2kmxFvx34Ad2UWjaf8jT4S+USeAylqsSpFjKRSwYmlAlznRqNXgGpuC08P5D7lBdUEuZMWFUBMiJ5xKFLAMglwuFp07RMhTQS/Y/gB6q2Y8nHZhjesXo/kn+etTppoZ/BWwadMmvPHGG+/nlxV+uGz96+I7WcngCfmvEHGylvzMrFPxd1Wo1mbmjr6TR7+UZqXhaVlM8zANE5JNv4tXb6ThVZYcFoRM22iNEBzX1BTcHk7dXjz0yqXuFWppKZTlZWBxONrbUIKFxTwl1scWA1UNHR9E1xCwYIEzpt9cH1f3AUw1M/hv46OPPhIQMv0hNjV+/Utfvy2OykgAi6VNsllPNhlTQn9TQjUyMsKYceOHnRerJsNaz9TXB2kHk1PleFmjgODhn4hCZRkYgm1l0+3FzSvf7YZNVVBVV2nJVF+AasgNPMdRaNUxmju4P8ruRhxEW7H8Agb2n+vs5MTUNIP/Gt58802sWbPmk9Nh5159Y/OHyCzNh4DDA1fEfyd1T/gGYtUxa1L8XQmVo9Y4VhpxlsBB+Kgy1apTwqaZKryhVMCigx3URKFyiMlPSbUrNNUWgyNNhLw2pfOdGuqhqqkiN8V+xKqn3ywl6nh6luJRl0QrqRLTP9WSP5ur1IxgqprBfwtCgZCbX1VoseXUbhTVVoDL4ar4YsHitL3h36uVKkad/l0JVSAQYNLkKb1vcxUTQNcxfVidUqFYpsHGGjm8oHp0VgAhNo5QDJ5d96lSssooFFaxIK2I7HQfdVMT1I0NdH3ADvmS+m2XSAihFmoeHRpoRoIhG4nGapeBw4cNs7O1ZWqbwX8FX3z5hdJ/xIB9RU1VeUIuv56Q6fiUXyL2q2RKJVM6f2NCZWk0xnVi9sQ6e0GHZj7hUIwoVWCiWonOwpLUPGcJhB3+JqkrRGVpNjJOuOFmyPtwMNcg4+b7SP3VEI31DSjIzWnPz3U1UFKTn91x7IySahBbhfdLiekvxaPzvlRUpfJRYMYdyVHD5T9ZsDNnzhRpNJo+ZBut2/rMnj1b9HvOtW7dOi453l93nrFkc3R1df3da9lGRkYGknOM0m39/m6NulevXobkucaTbSQtr2+++cb9v31P9SmlEUG8nvNjdlx8LvHHqxplnczP29ub9/+ZfG7duuVB6meErp6e+js8U7tOKeYLvPoumxtz059v1GZb65v6RSocLZJiCCHUDqNZ1Nw3MILpvMXgDX+wCFRjZTLqa2vBKfocvx4/izH9gXO3gClDyGlJk2ogIlRGONHRPQjCgN1E6HJgZe8JRWIcarZ/B1VjYzs/6sO4r2FjjJkQCto+lZpHXRRZKjjvOjcrr6rshP5PNTU175qamk5Ex1NwKYtXfvHFFxvef//9x57LGhAQYJKQkPCBXCF/Pac4T5CUlYLSmnJYGJnB1d5ZFuDh96lYKKb5ZMPJ9h6Xyw1TqTpyVANLly412bNnzysqtertoooSi1v3YlBeWwlXO2d49nDP69nDfROPx9usVHavdL7//nu71157bQXZ9ZXc0nzzyKRoSORSGAkNQM5TO8A38Fey2xF6WbIZXbx48eMJEya0vedr06ZNxm+88cYq8jEYra7srkHJ4v62bdvWrVixIkv/h+vXr/cJDg7+kBoZunHxkaH72rVrZ5966qkNj60M2GwWKUdalyukMunkqOQY5BbnQ6lWwdHaHr7OXnn2VrZreFwe9VcuIdtNNze3b3NycrSRTRogUqvV1LTaTDZL3XDcHcRk28bn8/crFIoO62DhwoXi/fv3zyJ1+GFxRannreSWOrQ2taTtoaZPT/+DXA53Lbn/Mo3OdUUGSzoNuzf5+CXZDABtAg0tF6pjPp82bdpvISEhbdeorq5eZWZmNumhdqyTQDj1008//fDyyy8/6aDET0pKmko+vq6ra3UHu1mSMpxIyrDwcc+7ZcsW3sqVK+nNvJVVlOOakHGPVVVfQ4xhFQZ6B8LCxPyis22PveR3O6pLjh49unPz5s0/37hxYwr5+w1dGah0/bPRx8fnhbS0tKKOrjVgwAC36OjobeSjSFeGlETo4vR7yNaHbIMfgx/p/hGkDE+RMkzt7vnapB/p2KynZ8/wPspVGoFHFKZc/VDVaDCuUgUfpQpcdselS9sDSyBoyz+VNjehqLQWdWl7wCr9FlejARcHIDGbnF7+QB43E3VpT7qWrOYW5Dd9Ye7zNfliFeQSSWsktEu4s9R4pU6JTVJuSzd+uKsLlOg7eXTfppALIZU11dqGX1hYaN+oaF694pN3jeKyUzq9Dl/Irzf0so5qTC/vkrQMDQ3JwNDgTYj07LHQ027fHt2G0oZqbfChFQq1UmBlYLpuxtAJuBofAT6Pd9J78VCf5D1hZfrnGjZsGMLDw4PVGvXe85GXXb4+vBXZlUUQch/YBUqV0tnbwXXjoYvHXp41auoE0iHzOrqvt956Cxs2bHiW7L/heGiI9XcndqCorgK89qrflMvhLF/61OzlNCi5NeRnWJtZ2JkGuYyqvZUr1T6f2GByYk7KK6u3rzNPL8nrtk6UpIPYGlsMtTAzUwp9rV6SplQ8KAelYuNXv3w3cv/Vk+Rzx8XK4XJ6WA/3uFoenpnQ3bUkEomhUCj8PKck79UNh7biVPRVGDxkJTXJpM5DvfrsfnbsbFy4dRUZpbljTCa4J7F/yjulJmyns9Z++GDbZzNOR1/R3ld3bY+UKQJdfQd6Lh6SkbzrRsTDv0+aNMmZkOmuqKTo0d8c3oLkgixQ0mw9r0qtNrM1Nn/1hUnPziVkPoF8H68jd9qB3l+z68vx+66fJJqA03YMXyz4MPX2rXQ6WNG/i4uKxyfkJn/y9eebDTJLC9rdM5fFJoOlS1B1SgntrlueUGw5V0lrdy39eKVxQkEGxDxBx+1+rMsm9q68paQMu33329q1awcQMt0TnhDlt4n0D1oe7bELHDZ7/IS+I8YH9eqPb49sg1ylMH/xzSVS0mbmvvfjmtEnb18Gn7Td1ufkBtutYGdmrFMrVRL9M3l4eHAImY78/uj2sf889D2ciKChZU+reunoWROoKDl6IwRpxbmd3i/d31AoRj93v0m9e/Za16hs3vb5R+v+tW79utpuCZXcnqGMx54AC92MqIcdA9UaTFSoYMTSdCtNWtFQnY/kM8sRei0MTw0ETIjudSDEGXaXNkZWG/M1SmkjavkrJx+4HPkuJs+OB7toOljl5WAZGnbrtxhJOvCmMtJunDntVSptSsZcVIowlK3R9KCX0AYK+EL8ELIDd3PTuus4VJENo8Kqm8c1pmpg++m9bh8d+gE2hibtyFQr2UhDqJU0Ys+VY61fmbE47KU6JdKqDCiZjqhpqD3zzcEfDLddOQorA+N2ZKqtOA4XtAO9v+dL79j0u/HNzc2DxGJxhv4+ixcvpmT6HFHK21bvWM+/kXpHSzS8DlwoSpUKOy8fbvu7qLbCmc/hTKDPpB2Ayopx/9JRxOWmP0JWHTYsQgKVDbWolTcNFXJ4A0kVR7f+FpYQhajiBMgViq7KntbV02TrllDJYHL5dNi5oI9+/hpShbzD+6Pf0bq+u2Ot/mBJVye7QrY6+ncVGQC3Xj6irbvHGchpHSTm36fk/xH5c5z+b4MHD3Y9e/bssW9+2Rz4/ZmfISL1R8+pf15CHqhorMVnh7+3Ph999VrS/eR5/p5+F8nzoFZaj09O7oCXRXvfv7xZNtBnRtC05F8jNqpr5UpioeDszUvILi965J6VxGJMzc8Wcy14k80CHA7UJBbVPS6bSqQSRCVE42pGgpaMusAsahXQbtzVTqveWTXigw8++PnjnZ+7/HztJIilQOTio2VMBhmcvXNdu2nVt6SpZ6OkcQEhYeV3pC/0NLV6+JBnyPYdveV2AzKbY6mEatnJ8HOwFhmg1QqhdXbg+unHM9/J/k0yCcJSYnH13i3W3gtHVrw1/2XHqWnTloecOF3cnQ9VJOOwBlPyeUR+ssmD12jQm1g1Qlb3th6bzdEm1iXcuQaOLApLJwMXb3NgYwGkEjqTEaOrl7sG2SWkFoig7ulIC5IwHRHul26zMXkoUFp8D7Ul4VCxhd0uiUV/9yM3PahG9ajxrmkJTpWJ2AFkl7ZcLktry+IrMWEnNBpNl48jl8qdnUf6DRQ6m3bVodlEnb5w+MoJr1YyfQK0OzGXzaXzdQ+u+mG14Z7Qk1oy7Qq0U566ddls9sfPHa+rad9fhHzB0Ga19LN//bSWf400ischwofuq2ebvysl5lh6UdZtAfeJl9ClCrdZ/4tL0dd+qKyvqeyKtFRKldDK02GExcCuXd811TUf3rh7c8AqQpQypeKxiFAPJnp9QG1hZP6ZUq3+Pa6zdorFy8vLKjIycuv6n78NXHd8u1bddXVfdPAhZG/y8Z4v95y6/JtzTWV1DXUldHG9T+jYSz9YWVtdTM1Oj+5G59S2DhqPi7KaivyT4WfXt5JRF/0D3s8NX0qer1Of3LLnl7l8/c3XH3514DuXPaEntEKD8/gL3VFVQi24wx1aCRK5u89zw8eyKOnoYG5uzkpLT+sdFntzyPXc1EfEiI78n2giBRUhDbJmfLp345Qc64YpPFuDboNSgmYuqxeMOe0VKks71GGKVAUb9eNpUxZ5trqaEnBrzkDEVqCOkOaskSqEx7ORUciCKbkXIk5QTx7rBjFy0nJpBQIh4Rwsm6pGFuF+TVMiePKTkFMf6GMUPo/wYrCSEKpU0353jZalkMFnW5CnbZccW1ZbTUfWjWywwojplexoYaMdITsAVaj23YjkKV8Ss85SbKQfEjuuUy5DqQVIth1UuHd2EgcHB8P4hPhV207tcTwUGwpjkVj/5zNkm6471zSdsmrz8xGF4jPundltZt3UqVNF23f8NGvjvs0OF5JuwUgo0i8R6kueTIU92WhK2Sadr6hTXDh0Rh51+ebbfKFgK1XrZBy6ZiE2jna1dtD3NdJGekmn5qkJfJSKE7Ld0z/X7cPXTlQXVy4nH8+Rc9zp7ezV2bjmp1OpHSInJ8dLBvmqVds/5XAeZIJodFbI2zRmqau7uWQ721Unotdnczj3nK2sZ5A/T5ItnHx31VhkkGZNFJpeu6B+tKu6Z4zUld2rbQO1pSUvLS1t+pmbFyf8eP4QrNsPiDd0iorWIQ0yHNK/p+SiLJu3flrzU1VtNSysLKP8HJ1mmIoMo+xMLDQKtVJfpYr9nhm2miPmaeXa/aK8BWpoNhBSC3eystO/10YOn7u97G7uaqJOn2iEaKyql/28fvtuY2OjtUqV8pK7jWOOh20PyDp+jdtCdDLrctrUadi5a+fYgxeOjN0U8kCp60DLcRHZntJZgrTO2uVRWhob4/MN3/LHThp/ZGLvAV/RPqpfFjq8QvlLT1nSjjN//4XDsBc9slQCbRtvkm00Oc9nTha2ddz28Zk4sl3T1S/9N4oK5YfOQf3K/p2a/NTEmDZnluVxrpLzyDinc8v2VKi1k6a6jw5w0ExMp/r6w7DWnIeBM1Gb5HZyiBq1s1RrmWdUX2KCiR4oXUqsO3/jYf44BaoJ3VgSbSQkYkrhWEiI1QjKHD+0dBhNF6M84E0Jv05D86nahxToYSw5RkyZZH/xxCk0NLVwR+bh29RMeIfL5fJTFIpjC9a85He/NB9i9iP+Imr60mh4cWdjSFxmomleTUWrolRyedzvSpPyVlVFt/PRnB++cFxFk4HmnXpJk6CDUdq8vKFyxSdE5fYwbBOu9VwR/43sywl7Jdnt6jXEdU7/V/kiwYaW8UTDrVNJ5jhN6b0h/7eELAGP759Vnrd4JzGT9Ei+mS8WzEw+GH5R3dCuY4T7LBvxJSG3oyW1lcO4nYiN8lvZaWR7pe2g0jLf0MTw22sObqJRdcglspqco7HjH6fD5p9NpIPN8cb6hhk7zx84GpeTyuE8quKI7YIxOjJ8BCKBaP6mQz8K5aq2DqYmnelic2X9LHJ+yUO7H3Uf2Wuxma/DpvqGBrOOFKNGrVYm77h+qtXNQTF8TPDiEQuCf9554Riszc2Q/lvsWnlR/S9dWCvGRCz+Y+3+jeBy23yfclKHH+VdvfdlY2ZFu/ic66x+E/kGwuO6wAlbIBYOc53W9/mc0/G7k7dfOzXu6fHVM5bPOPXB3g1m+tYKIdWZ3ouG7UzbHXYh48ht+l61VeTawndC9ko+3rdJO4ByuJzMyvSi1ZUxuRW/R3aramQVSdtCV694ebnB1q9+/OHlr99yjctJh0DU3pWlkisDvJYOc0vbE/5I0IbP47lK1LL5IZEXtZauni/4VdIOt5N2qM+OYURpbpn43rxN9wtyV7QOklw+l6Mul8KlwSzB1N+3MfViriFP+IC/lTLFEJ+lwzxSdocl0rWWKysrHe9lpy7dG30ZXmbW7S06MT8051z8r5L8Wm2w6aXtc6edSrgSUNNY39KAVOpXMw5GRbV7BjsjG69pA34gZT5HV/a+3tMH9U05fDOJlFGHCpXeXYCG04E9z2qJj7lp1BB1Y1fQwtI0S6AoLSaNiQ0eOauCEJs14YbexJC1MydyiJKpuP1ljA2BFTMUWneAo00LmVLr9XoikJFP3wzARneWHH0QOw25mEyDR3am6ofPhpLNopHDR+xehUJhkZSdMuVUwk2teabUqCSWRqZ5ZESW0hGZmBW8nhMDg4jM79TeramvJe2F1RpQuV9xv2jrQ2TaMpSPmXPKydK2/GE/tVgs5hcWFi7+4dB2rYVAy5It4CrLk/JWk471MJm2DLVHYzcT0/hrfTNdIpOtGtC3P44cO9r38PljFhWSJrQ2TNKIX0w9FvUwmbYMLvuiKvf+64ezxbqGRYulO7/YHwEuh/va8bCz7NZ7FHB5UlLuyTTgoyR9zba3S5BZgINHh6RcWvBsRHKMQE/d5svqmyd1QKZafLn849+WjJ2dJPuTXpbL4/FYZWVlXr9cOOqRWl7Y5kPnCHlbC26mPUymLXV4/M55pUKxSE8tUHW1/HGD5mSz/rPriMfl90ovyRoen5kCAZ8HqVKOPi7eaUQ9N+qV/RsdkrJG7Uj61pAbqXEw4gtb2+HHpB0+TKa69sBRnF3/ywGiyvGE9fSRjq1oH5127NppWAkN/pDnl5c0lJlkqU6MHTCcBjdbv/bsiEva7CQ5/UTXO/Ucit+mEivNuY92qT5tPTdr0IMoVP5j+E9pZoK06BoaMj9B6xo6dIGq+wWAB9EbWcTyeGszH+9v56OxmRDptwK89JUQpkbaF53iZ6JFfr0M3CRk6u9ElPHwZCgsK7Wk2i2h0ruTk83WDZufWoa3gmbhp7Evk2ei+VkaSDgaL7JLR07J6ecjL8OMJ2w1//KMRAZrrUwsbuuZT9Tc9unQXCT/1Usa8lgstja6bcQTuRzctHPlkV+PPHKtl5e+GBMbf/cUIV25nhlIWgKL9r5hp6IuwUDYppBpbsz3XT136p6wT50t7bQmK9l4RpbGo/o+M8yoSdLkHns/oe2ZSCP+KePMnVPKyg65hg4qSq8eHrsGuXlW0GcgoL3/7p/ZUYuLiu1u34/rV9VUx9KVu5SU++ax/YIPNsraRn5/ndneDukpaS4Vshpxk1zyIJ4hVb6dczKu0+vNfmZOzZYffzxrZmTcelA+Hi896rHHB+r6OXPrMkx0LhZict8puZP9Y/29kk4PyjoUfdzbzuWurg7BNzNwdJro799tJoVE7ur13PAXWRz2n7Za1YwZM/Dd5u8GXbp5tW2QqJE0108dMv4VTwfXDD2lv6Sj46vrasxTctIFjeoW/zZph+lE5Z8h7VDZSTvU8AX8e+5OLqd0ZC15nHZIhMVUzyVDTUlnpP3og91Xj8NMKP5DymDB/AU9wi9dX8xSaIhAVHXLQy0NgcXyB4vsnJ+I6xW52reUrg6agVeHPkPGTFOw1apuV6PWsFWQGuYhuSIOmw8RcrxA2IK077hUmrZCOjW/hVRnjZBj4Tg5jMgAMn+MDEsmSSFX0kgsYEbKoJczUapWQCnh0Xt5pFJ4pZByG9FtvEClwQi+OZ517YNXEy9jY9gBvHT7FBYbk5PZekCmVhl24ut5/UTEeZqHq/2jSS4vGerbf+fK6c9fkSjk2kauk/l+HDNBR0ExlbNNj300JUMb3W6sFa//edM7KjN2aV1DXTQ5fhvZFs2ePdtcd8g/qN+IdLaB6XsjLrZEZJXsjNIc5/u1LSlNbB6nrjwp/3xNfNcpfjwBXzYrePIN5QOzlw7LYyUyqUtFbTU4HDadM47sywkR0sK65i7rjwjtisa6Z8kzbNXdY9Sfqnx4vJW/RVwQqh+oHCmxDH5a9vTCq+ZiQ62vjJhzBo5BnkMMvK0fUk08k7vpiZw2c1+tURPT92R316xqrvtKrlDQ6P7Puuh0/R/4SLQ/OUWkxWt9haQOUZFccK3mTv797g58cfKi0w/VYVAnu1K/bUFbk5cqPvNeOswerD+njvhcnmOzSjo2POEWTAUtg4Szjc3JhZOfCa0rrr5jKBRrGUajUgs9Fw1ZpH/syOCRwmtXQvtFxt+C+EHGC40FJHbZjdXqhpjs5HeFXD7NCV6jq6tHfN4mYsN6HpujagteqTVvKpSK4fvO/2pM+622IMk9U7Xbgd+1W/Tt29eIXGfVvv37Iraf3jvxOB0oH8Q1Glps9w58qNBOktKYkuEDc3xG4B2vIbhTXYzIqgJUK+XdBoVUxESNrs6DxeAYOBEh7Eou845Hi+/0ThqQW0pNOTbcbNXaSP+oQOByDLDxMAfzR6tga0nujKh7CSHd5Dw2JCo1LE0IsboCFmZ0jZQUaPgpyE7lwDLjKZjAtEMXACV8TX0zihUybAwYjV2WTvjALRAnKnMIKxcSq/9RlatSqfwv3Q71zq0phykZ0choX8eukh5b/fw/cHBgr9jgpwfU3bmfbKIz38bqgi7t7G+1Wq0Z4N33gveyEUdJA59DUzOKCTF+cnCj6P19Xw3wsHIcMLL34JcXvfE8vv1hY5yTreMRFxeXLXl5eW0mtUalYZdVlTuJHwQry7preK3o793nhlSjCuZpB2f6mBzHmvpaZUReWmvaTf3DkeiOoCY9Onn7NWIf4PKfbUbeS0yCkbnxtLDkaL5OiWjUUkXs6S8OZrjtv+y4fN1b0T+dOzLQtMVXRidBDH6I4P30AxGtSr87VERk0e2HP+mxiKRQBxRLGtCzJaOiWBec6xZPDx1/d/nmD7TpRDpT0q9DVapRnbEyMLVvkkleIoNJqzLdrWubf7xLhsvzyS7OnXw1NVbfF3+ZLtPZz6fP3trMqKkNkiZrnVKlKnW/vguwSSVBXhnNoea2Duw58qJ6ZTftUEPaYSb5+Fpn+1B3gJe9656Uwqy5UmmTnS4eQYNFwZtP7SF92UBrLfawsFH4u/rItl85Zmgu6jpDhbbDff/8bnPQgQEN5LNnRW2l3Ynrv+Fk2FlcTYmBsc7qICq7Ku3U7TBVjUzZqcnP0uosBY42VKNIUg8B6diTbHvCjUbJiMpRd2Hsc9gsDLZ0Qc/7s8G6Mg6F8U44EQ6U1xCW79niOy0gBqRErss31UDrXx3sr0J6QUtWllKtXV4VJkYamJHx2cMBiCVkfPIa6QQJLkDoaLhlTocpy7RLf2odeYZrTbWYau+NzX7B2kIaZ+VETmwDeccS942QiAsQ8Xj6RKYNSgS4+yYN8et/s/GB34RGaF06uTRVf5+QrVL/S0rEBdVl2H/tFP6xYx0Gvz09cNGny7/YsOeHGHJvzpzWCCMLHCFfINZTa5S0sx6n0Qd69iYDgxr/SxAKhCNOhv5mL5W3mfa0cWqJzsjAqLS/V599tBNqrQOp3Nsl2C9Y5GL2l34meq/X4sJh+iAJXoFusiceWAea2pG+/aGnUjsJvnLMapob1hAybZvFplaoRnktGfYUuf4f2ghGjRwlPPDLgeCDZ4+0ESQhk7T00zH3aGrlsxPm3Ax061XI1gkV8vtIj3mDLNrIhexPFCMapc1t8YU/rP0QC+B6amzsAHf/FCO+UK27vuknO78ILm+saYkbqDX1JgbGl11snfKbH8MfS59v6ddvBXovGRbss3S43Yg3Z+Bfe75ETNY9GAvazRqnbrjYrkx+aMNNAkPs8eiNxJpsjDv6GYYf+wxrb5ABp6GKECz3sW6IIzGEkdwOdrZihCcQRigBPAmfuTuotX7R6rqWnFNKtMQiwog+LX9THqFmv7O1Bq72wE3SXGpIU3S3AxzUDhDXW5Gb7dqPSkvV3tgUk4zM4X5xC04Xp6GZKOwXw34hrJwHIefRZyiuKp1+Ju56WwBhcuBIu+IrKbtpWtDhXw7v6e3k5+1qbqtt6DRdxXfe0EEcMe+RE6nJQ6TtCU8ho/CwrhSeEU+oraB3dq31fvnbt08TS5Wvi5spGyVNJb+z4ZngfwwCvuClkMiLpjR3lMLGxJyb8mvkalLuoVnpmRf7uvq/ONCtFxrkbYMZTTWy/4s/lkahUNT9zjrs81gmuFjAyzgeXVlXWLmOkMaDwVut2e27dLihUvHHrbfC5RBLR1r3dHjibRgRxV3SUIulw2caNeRV/kDryd3T49Lc0dPtjARijR6fvPYfK2y1xkgoFH7NYrHbcmx/DT+jn3dKsx9O6wJ9f0yZiPjfZJyP+1xR2qTozImuHUlpDhuk9cueS76J0L6TcHnOahjw+GjgqLHmyGE05xIbndPtO/3AY3EglvaAu0cT/J++h6vksGYJUaO9CJkSw/My4XUfZ7I5UWUFSIlAuXmPmIBZLLw0VYOY9JaZVFShBvgQLk/tAXmxBSmk7gdfGreKFKqw3twRP04agXJpA6lhFj7s9zTWVodApi5tt39TQ+PMQ9dPmNEperoGhOziPKMVX70zXknXKyCjXGNzk1Z5aGX0A+c7DRY9kthHlAKIqZLOMROMJ8TrpUuzGE22gWhJi2mnXG8kRvcmymIGIeLD2s6oVNShZQ4zBfX30rzZqu6eu7apbiTnz3Ki/Qm4HXXLuJ7VPDCvvEj70hx67wqlkvXqN+/2V+usCBURW3XNDdCbSEDTsfrryh46P+Jfah1R0ofUAZ5+ifVKxVCLVr9eiz+0+2PB6p2Yl05n+HS7r7pJIQzdcPzkF2e2vBiRFjeBx9YmazvqTN4/BHTG3qUrl32PhZ7uE1WUqZ2hZEZa8I3ESIeMwmwH2j+ogJIQ661ZIdULJ2j7xyf/ifI2MzMx2PTW+osz3ny2MF2Rb9Yuh5zN0tSX1MTeS719dvTgkWv/XcuDYuaICUfvXbrzaXJebaftrq21cmgaT5MG/iZW6GfmgB8zouFnYo0GtgqRinoUEnUnocvzdRPpp1aHmaEVNHbPI73sNAZ63tC+8smIVEZ0MjCIkGQTIdjj11umomYUctDDWgV/dw0OXWVh0VgN/FxaFuNPz/TBIL4/IUVFt9kF9Pda8j83Dh9WAjGSa8tQp5BhlLULUcKEMGXNNPJ2R9//ScztxSfCzrJaGzGdfplclKXdOjC1tP8SkhzkvWiYS9rusCKVosUfToiXo1Kp6PRRh/j4+E2BgYG3k7aFppG/P6ObWYAD135QT1elTEGT+5+l/m69sqevvT5M+ZSYv+FE3XjrvqdkShfISOnquaXNkoGX7lzvzX8w2Kl1ZJ/xlzX3+cIlF2+H2jTQlC7dQFDdVI8byTGPKlmd5UBT1zwm9B1MzM1z8pIGZQeE+ldQ6WpbM5tEpbotEmytU55dznWsr6/nFVeXTq+RNLbOmafP1Wk0kmsjZvn26QWnpwNeEVsaR5EOb6Pr+B/fz8sEj8v5tx+Ey+ZS/8rMs7cuw0Zg0GZm03UculrLgSPgWrvN7Dch+8SdC1evhUoD+/W7M2bZFBTElP9JMpXIT6FoCyH37x7yqVMhsksnTJ4EO3Vti6pa2l/96cBByfrMzStzVr2wIo+jZv3r9PFTiq5MfhVHgzwqF5NyYhFSlIZXPAfBQihGfwMz9DM1QylL81hygLI5l8UnJpwDHHs4QkOuYEGa+skbLaY/nW5K1SftJ4N8SY0FtzS++PtsDO2lwY0EMqQLKbkB/YIGQmwaAK5Khe5WEKAsUkhU5F0yjI6wdAafkOiegmTYhv+Cj+POkOKtg4mS1RZfKy4uNriXnzYqr7yYxXpyE22yvhlByHTZkasn31myduUzG8/v+NosuP1qcTWJRTRhPCN9383vKvLLBvo597wn4vF1wSi1ndfCIY5sDVs2KWjsGSEpGFqGRO1aWfs7jTLr69h1FITN/uh0+DntHOVW4a1Wq86bGZsmDXP21uYMEjIydhvb20/o2D3neHp6GtBl/S5cuGDxZ7T/307/hoDA3hOv373ZFol9AtAZYr70g5une27a/fSSVv8dUSR9e84dZP44J7l06RJdVnGIq6ur4A9+PCUH7OtTfAdARp6N1CHXyq9HsFk/J7OuyYuz+uzNS6Z6s72o37XbDIv8s4k55mzxDnKctiBppz8Xd6OjqZZPrrY5LNuq5polIXFh+rPsHsuboxMNLS4KPr/aQGQgo1N6STsEaYeD+A7GZo/RDoWkjoaePXu2dzeEytv8ztfbfRzdH25MuWQLfVJzX61S7yb99FOy/TPzePRwvlg7M1Drk1XQhVU2f/Ta6q/WzFq4YCE6JVQapT599Hj+ACn508wOw62csSMzVmt2GXMFuGPMRxKxKiQaVvfz6gk5KeuqYMAyR6+JP8K3zwxExJNhmpj3dqSLuju25JuqtSMLtFH/yjoW5o9V4879lu/TcgiZjngNbhP2QiA2gfox1uGl66Fk0McxMwBdzW5M1AmUk0a9jwwMsCEEp+Ei9Mz5Ow1NTVr7hM/la1N2VA+IWn/aZNtGfanGQoMIMV9Qo5fETKdT6ndel4/3b8Ct+wmIybznR1TG4s7u88xnPxt9sPBNoVgggt75XGQKmUYoFN4e5BeQQ9OvaOcQqNlLQw4c+8dXn3/Z4bmqq6q/CkuKGh+eekdb4aTsNcoG2f2f3tvU3H/IwFRv957JNMjVTJS6ldDk46L7ubOmT5vesauGpyVy58SkxNNXY2+EVrDqQt/++F3XP5pQRUJRn+vxET1pSpeerzHh4XLXlf0VMvCkG/JFralrPl7TBvRqTV3rYWl/hstit0a1OEMC+h2vqujaQ3L8+PHFw0YNv7L/wuGbB88deV8sFv9hpKpQKMAX8AutrKz2aVhoDaiN2r95x4Ytm37okOXIwD6xWlK3asOJHSy9xXRoSs7Nx7lmSW3latJWiv/IOnJwcODFxcWN2n1qv/btxjrB0agLxIR2UE9XzQ2MWy0/Dt9I9JTz5N7ahxEJRGkWxqYnKKFqTXKZatGhXw++NGhoUKdBGUtLS2F6evoHSVkpEaWoOffKe69P60IRs80szAiLczfpDFV6v83S6sYDldcyIDAQsH9vOaiaFHWpRyI/IqTalrblZWYteO7rt94+HHrSq8ugFIFEpGHlorICV8qyMd7OHUJiCjcShgty7IF7psZooPOdu7sL0rGVTY3QVJSiScmH1O5DBAyeDydLok6LW0x/ap0ai3QTmqhoJOrXyrSlOKaNMkHg2HVg2y/Rhn25RkZ4nOAl1bmFfDH6OTsjt74K061dYU3Mp8UJl4G6CvqCVFl1TXXVtWvXaEP3EhmJXjwXc601ZQe+jm6FEV+eXEJGplH62+weI+dFb7+4/P1nXrtKl/KijaK+voG7eM2KReTYtsVDKonJql1TU6kyHxU8YkO9rPG1/Xv3t3Wi7du309S0gYOHDQklhOVdL23Sn9OsTWnicjmVHrYuq+lC7hztikNNvH9sXvPF1MWzNv3jnXfbJglER0f3JOf69V5B2ptv//gJ96HzfEU/FFaVJMfnpu2zMjTV5kQmFeUI/7X5019+3L39G0qcRAG0GxvlcvkkuUJ+bc+5X0Y/8/lrWH9kS8CBKyf+qb/Pvn37eugWBKYLUw+1trEeoKGL1+rQw9aOlmdv3YLBo48cOeLb+ts333xDF8l2Hj1u9Bunws45VTS0ZHHxibI+vXbv2ofL3S6fN4H8u3Lf+5t/tTI110iI0qZq25QrXtBQWTeol7cf++erJz5pUshaTWNWbNa9EdvO7Imji0pfv369nd27bt06qrzX9B7Ud/P81S9YfHRgA577/M2PXRYMCGg1UQQCAZumy+gW8qbPMPDAgQM+hcUt7vKkwnyEHDvhQ74fpPt9HOn87QiZDIaN0ZlJW2yMW8ZbYi1gw+FtS/uMG3Rk5Ssr215uFhERQRcg38QRck8uXfeqqLUdkluRyGuaNuefT8L48eP5F89c8CstLeVTYmuQStDL1t1RXto4dmTwSG17oBMZ1FLl8/iDJiiQAYZbWFg4paiiZMmFmOvaCQr0ulOHjr3Xj+2yiNTJaP16+u3DPZPIv+8vHjfncuszkH/Np4yZ8G1KYrLN3Vt3KjZ9/M1BD0dHOR1A62XN2HJ89xffbtl4kOw3wN3dnaMvxsh33mVlZZdC74R9+PTqJfjy1632BzNCX910+EfbzRt/cKmqruZSEVQvacYQt97u5eXltK6sMsvzqeqgOatrSJv6bNXilVfI98OvXLrSr6y4lEff20mFxSAPfyuekm1w9MhRmmPa09/f30ilS9an9//qtCWB9TX1bTnAymppVfrpmPcIqR5tszhr6gZMWDrtLYVGOTQwMFDY3hfeqlC4XJNRz87+8VJ/+/m/zniOqiPMiDhEumgZIUlCaKUa7CuQYLRKqRef6UQ2S5thPHI8DF9qCfjJGorRKOPi4r4lcDBIgZt1PnacBhaMaVlgOinXEgN6WUApHAjL/l+DQwhAZKBbUzXkOKr2/QSOcdfmai6pjMH2Vli9aB4+9hiKm5V5iK4sgo2BMRbHXgV2HI2M//T7pX0G9v8+MvH2hIu3Q3Ey6iJoYjgNSo0KGIxhAYPq60qqV728+EW6iAlWLl/hu+XHrXtu3YsZGHY3EmduXyGKoEqbJDzYPQCzRk5RVjZUe784ZfHztvMC/2XxIE8PvRw98OzY2c1P9RueamJkQqfo+d/LTjXfe/YXXEm4qc0+oP5DFoctSdsT3maW0AUvfBcP30bnamvLTqWAs5kN5o6apnxmzMy75sZmvIraSp/DV07yd104BD2zWSVvkh7OOX6nzdwSOJl4uU8K3K1qlg/RliW5pp+DG2aNmIyJg8em25pbl5JGTMet0beTY0HOiaOkTFpXpeLwuZdK4nOm1NzJl2/+/oehr7z26u7L0dc8c0sLtC4YOgEhJv0urife0qbVGQnEeH3mMu3C1bRx2lvZNh/be/gdQ45oGynHVXmlBesv3Q7l7b98DCV1LUrS0cwK856ajr4e/heC/AfQBaLpIt0GdJHuzMLs9y9Gh+JGfCRuZSWBR0ZiE6EBloydgz5+vT+YMmjcequJ3ot4YsHuVp80rct+Hr3w6swXZN5OPdONDI2oeutZVVftfjHqKnZdPIS8iuI2Fwl5xrmpu8KOa4PGavXi7OLcvTfib0KuVJDrcVFVX43LsWHIKitAAynrxcFT0MvFm67TSS079HR2Lxw/cLSXSqlsmzRBzinyfX7Ee6QOP241xYWkoc8aNhGvzHohxcrUsqpJ0hR4Oea6weZTu8ng17YcrobUR2x9Re1AixzYJaYkHbwac2PU1tO7EZ2ZrN1hlN8ADPQNRC8vv/qFU+b1L8ot1PrKvRYOuUw65ph26o3LuVuZUTyuPDzzsebys9lsHiGX2wkZ9/qeDDuDbRcOtyWyj+w1EPMnzVF8+cHauRdCzmvTCu/ExHL79g+MIffYp6CsCF8c2domEuhCKq/NfrE4PSZ52Rtvvn7Bd9moTaQdaqen0v5jJjTCwjEzMax3UGmfnv4ZpD6oRzEwNSfd9GzUZey5fEQb06Dtv5eju+zjJe8qZXKZwWfEEkwvzYNCqcL0AU/B19ULLjY98PSw8U7k/gtoGiKxFLwkMknalZgbyC3NR3x6Ii4kRYFFiGuQqy+em7ggYVCvfrbJWWk258i1LsWHa1cqo2120cjpcLRxUCmaZCEvzXt+Zpvf39VssPu4Pvvp6lb0/k1FRpg5ZDyCBw4/NG3EpBXEWqxrR6hcIo+mzpu94IQjf9/Tc2ZiqrEdXr59Gv/yH4kl7n3hdeUgFp6LwkdyDYzZXXs01aRDGfYdBKOVb4Glt2JSdVUleKQg2VUXUZr4IzT1F2AR8A2qNZ5w95+CqsoyWFi2f1vq4xCqSut95uLjQEdEPvs6UYsN2jykejIi1bFVeOXkIfQMifru7Obdp5KrsravP/i9Z52kscMl1QjBpdTlVwaVXE1tePHFFyeMWzj517e2fmpCZ1F1tL9CqQjPOnj7utWc3qv1CfXxfVXsLwihvt+uYZvy3f2eGXaUdMi+T3CqcEKoIwihtvvS0MNqnPPoXvtIQ7B5kvti8zjKqrSi9eU3s7SksP3HbS/KbNgbvz22w+BJfHT5DTU3ti774M2xA0e9u3LjuwsSc+9DwOvweIVKIh+beTTmRr9+/awuR1z9aeCLE6erCVV3tH4rTRJP2x3uplIoc3o+O3gbm8N++UnLni/kS5N33eihVqnpUoJsQpAxHguDAmmQ8kn86oS4vk3ZHbaqXfkZ8Rz9nh1+gNRh8BPcUolcIhucczQ2r3ff3gM3//LT7VH/XAJ3E4vOUngOpu0OW6lSqOqpH54Qarq+z/B3ECo/ND5cNvuzFeisLROlFkYU2/Pykoas9JS0Zy6lhP24/dxB084WCift6GppXPaYxrI6e88p/feSdvhEExCaZFLlrMHjyj2d3PkfHNhk2dnSmORZj5M6mEMXyieEOueTXV/8uvXiYZh3sPygdpaVyFCbnaDsbCopm1XdVF63uPBc0lkdoYIQ6nJy/9/rsjceXFvIG5W2K+wGaUeaNgteqVSqzh09FdejUY2zeUnwNrGCbMFnWEYYvU7agE/83HDPxlTr6e2MTKlZUi8ntrWSEC5Rqdp3m+jB3MISRgYCGDhNhdhxQsvNOM7XkinFw2Sq8yCgVi5HaWMTeXh1hz5c6sG/ZWiIEQMHYn3yTUyNPIopPXzxrFtfnK0m5lphAbKu34osLim9nl9ZVEALsosOQ3uvtjURs6csKSc1nyqMLvYfrvMrPTGIkglJ3xvx0SPlWCvPSj15azFpvCmPeap4pUIx5mEybY3BoGWKZf6TBFcoh5Lt49YvIu/FpMRlPOEacC2wEHD5svS8jPSc0kL94NmjmUMtWQ1UtzdL5bLUzJryDslUvwjp/zIORi0nHeDAE94X9b3OoxZcaz/LKcvfV0Pa7e8IUj7yVl11g6Iw9VjUElKHVx/zHHUqpWoUJVOt76aurjQ69U6sKa/LwYtaHtqIUfqByEIyOH/67wXMNZrwu5E5oq6vSddhtWuxankXY9Pv1iu6jnEYsAU8C0VxQ3HBzbTlXAHv5hP1EQ77ckNTw6LyqopMHqfLtuCsF5++eyMhCgZ8fqdxHupyU3Y9L59yY5ubTZpTg/ywlJ2EPI92cm3Owz5U0pDV1T4yXEXqfQTf3Yc18T/B/cBaDDz4EURqc9z1dUUIlw3JQ0uOFje1zIRQspWosC6F0q4B6kYpaVSdT5MWWQVBJfCEsWnXudosutKUdTOa3SqhECjIddgorW/UTndtxRUND+dtVQjLvAN3sTESxr2ApJoSsI59hnNhIQiQCe87WtrkjBw7Cl9v+/5LMkKVdZ6EATrlTft7aMLN+ONRF7cai8SdJjjw+PytpIKu25iYtTquqUrY3U07aSAV8wFRpnPJvXR4bmVZ873Mc3HBHDF/Kzp+40wrKXzF0mBU1qHoDkPmdIWj5O3XTpYnFwQTAt+HlgWfu0hCQQ7pmM/VZpe9StRp2w8/b9l189zJcwfEItETZY67WVqnvvjK8txpC+dsVXNZYRx2p154GtTQ5pjG3olt6uHidMC/h3N2V14e6C1cnf5zxGKihujI/DjTdY+Scg1I2Rseonv9CVUtGnd711125hapv4OLOlzARlkpycs4c2cWIVVqgVR2cfwxDQuumb9oX22iRUF5UcHeS8e+6cbqoa6ptqR2MjhvJHWc928wqvKXsN+e7+x1JzrQOrqny7SoCYuO2kj9vl0MzLd1KUyov1eSXRyXPYnc40d6A1mnfYS0w38151fP/PmbHaEn7lzeTtRmV2kh+3TCEBweJ6dC3rDq4TdmPNTW4tD1amp0NZtb7W4otUxZHJ2xmQwKafqWle5cynYmv1bW8oWYOmfmoiNm0n2YMxlbbX2wMvEavvT2w6myBETRt5Kma3CmUoq+xNBu7Rrp4nxCorXwyPYBJiVD1ayCScQkmKxYCm6fjl+qWVtbg3tX12DYrE1dl+qvJ5GdtxLwUMDoyiA0DkhHZbIpeta4gE/uoJS0xJfEIkQPI8RMXzktJ3qVJ0RP9wHwNDbF2UsXMSW1dsedKzfeKi4r1U4DdJ7cWyiyMOpHV2fSG7U0stqm0pzT8ekP34NpHwc7hwE9PZQyBVdPoHNIB84jZmcW7YzE5OS4zx3okHHollYJ6tZQnElMPhoN9NQVeDbpXFEp+8PPqJoUTY/bzk397Xs6BHlOINenhUmXsqNS9D5pmMdTd4eVadSax30rDexGeTmYuFiP16jU1BwYiZZ547RBVMmbpBeJyo3p6njbkZ4upq42TuR4djdmMKfiflFJRURWO5VtM6JnLwsPOxuVvjwgozEhg2uPmJd2RiJShnSChIVe0IVLyrA0ZV94uqpZ8chgRDoTx/v5EX7ENKOmdk+dotJO4yXHpd4PiQ2RFddXdypLyH37LBvhSY637y7QQ0xufio19ZQqWXflbuxra+o4zHuqSqoI0rUHmuuYRTrnyZRdYakadcfzogX2xiaeU/v7kzLg6w2sPDoFlJRBESkDtV4bZpF791DJlTT4pSZ10EBM/gRi8j/RBAhShkJShiNIGegTGJc8b2HWpbsZRK21KxfXOf0N+CJBP32BRgZOdX1RdVHh5eSsTtohh7TDiaQd0foJREvOLl38OZs8xm1JTeONvJC77crVxM/OxmGolycpQ/1+yCNlmEjKsJyUoUbP+mP7Pj/CnmaG4EG+MosMpIqcc/HJkvzaGsexflbGDuaexAJtx7wqiVyReTSmUyVtOcjVy6aXkxPp/42kDu6SOpA8EpRq67wGhr49F04Lielv5/6030DYcwTYkRlNmmRRS3hezcLTSXJ8JZfCvPWdUGwpVG//BpXOU2NChHLBvrEYNH01WMHDO624QmKKOzr26NoHF/ETKbqXIRNRfwyxH0jXav52PEQ1xqBdeiOh1W8H2qHq+TWwOEfEXCMZ+GqLW+6VUJhDjrJe9GvoosySghAwYMCAwZ+IR6ZU8AWCmn4eXoIUZeOYDEUB4gqIBSQlytgzCCeGzMVLfUbhw8pkmBY3YwCrxadJp5tKiWnuPLYcNGWXBujVHtlIl1jA3Gok+PyOZ24YdxFoqqqswKWzr0Ng/BlcyRgmNtSmyCL7oC8M820hJtc8q+Lgny4iLBwxBpNt3OEsEGG+sz9OlBFr0NqBjP+FGFOkOlmcX/B9bX2dnKluBgwY/EcJVSaXqyvzi+r7engF54s1VuCqsGrofHzhOQAToo7hc9+R2CApRXhTORyJ8g8gpKohqlVeaoAy0wxYEmNDO7WXLvaijER0TAkMjPrDxOTxIuDU8CkuTkdF8XsI8NkP6h+XkPPRRa8KwixhctsfRnIBrqk5WGPKhe24YHzbKxh0cedhVs4okTRgrmtvxFZVoDoxs5F7OXbD/YLcO0xVM2DA4D9OqBRCA3F5fztXVZJSMhl9/PHPnoOw9F4YGgqSMcdjAJbbe8LUwQ4bstLg0KhGAFsNrpqHpntOSGVnw8hCg9/OtLzKJGhAPBrrfiYkqQCb6wyBwATsDhJZ6fJzeXnJqC79CmzFEtjbJqK8jCpVwMickOwdSwjPD4JJkwEiCJl+acJDoo8h7k97G64HP8Sm5OsItHaFp7ElIqsLcOS3XzE7H3uLykrXV9dUMzXNgAGDPx2d5oc42NrZB4wN3nbey3DKV/OexzN2PoivLsa0xFAg8xYiF6zFkBNfAoUybMmTYgZXqXWZF6MRdcPuYsjcEkSFk79L6MrdAH0vHiVYlaYPqmt84OPbjy5eq73WzZtX4NwjFXJ5JuKJlrQn1npCAjBrJp0iChSdCoBJkisswcc1JRubTIhC9STH0ukPRlb4Zfh8LIg4rJW3u4bNxbID6xGcx0+uiUxYnJiWEsdUMwMGDP6rhErh4eg80WzKiL0xTkpr9B2GEQJjhGXcprY8bk96DQ10kdfyPKw9shdri9SYx1LAlJyxUaVEnW897OelQ64oRHwSMdl1KWGhYS2r8Mtp3I3woaEhXW2HEi4PJeXA0MEKTBpPvicmfnXeEEhDnGGQKYNayME5JRcraCxwxCCc7zcRX2XF4Nq9azQ0i0+HzsNoW1cMvU2k8YUbkqAbmR/funf3a6aKGTBg8F81+VthaG6W2d/ETpUolTwFFwfOh579MNTRF88KDDH5fhT2xYRgw7A5SDbk4md1KYprNLAmRNlDJYNdj2Cw+21Dk3o4hGwJLCzT0csXMDUBHIkC7deH7OcIGBPi7B8IeHsSQp6jJqrVEhzeO2hib4G9/GnwYxOR3dyMHWwxPnImbO3njKhRCzHq9Lc4HPwsdqaE0yWbcK0wFbtLs4DYu1iYqTxU3FT3bmlJCVPDDBgw+GsoVAovTy8E+gfsPOTKXgZ/V8DcButc+uCDyKPaFf5D/EbhIluDURY9MPvqz0BaOd7Nb8bMQYPQ//W3tIs2U0ilCtRWZ6GqIo58LoevNwv0rSN19RqkprPg69cblVVO8PRya7t25s1wHNm1Cz+J5MjztcCGMQvoy0u1yXil0gas8h6Ky4RE58efAwrTgSoVZic2RRYUF024fft2A1O9DBgw+MsoVIqqqiqapXvpKbaF/z1VnTfMhHjNMwh0qYbkiSuRKyHEduNnjNAY4Oycd1BizsMPyizsKixBRWgEFA1NEAiFMDMzhbGJNaxt/WHvGASeMAhsXhDEhkFwcgmCSOwCM3MTFBYWIiI8HDv2/YxFsVdwxY0D46dGIWnySpgJxDibcR79rdxgY2CGO1XF+DYvEcvsPRERF4uZiZLE8qrK8ZGRkXVM1TJgwOAvp1Bb0S+wn4Gri8vpY47s0RgaQGx3a2QEzcSB3EQMMbdHdOw1ZDs5wUdkgnlOvbAi4RLOxRDl2MQGatSY2shBL0t7eLq5wcTMrN186YaGBuTk5iK7phw3FLXIseICxjTvqgnLhy3Ac669YU3U8Mt3L2K4mR3kaiWWuw/Qrv/ncHUnEHcXUxMasxrr6oaGhoaWMdXKgAGDvzShUgQFETVpY3fgiAN3Hob6cGFOM+1j8f7QeVjiHABLoSH25dzF2ynXEWjeA/NsXFGnkONUZQFSqgsJsRYRG1/+6IQ+ehfGrJYkVGNrfNtnInzJxw0FSbjcUIZNfSfiXEUeJlu5wIQv1K4WszTxcsurUyPjMT2lKapZKllw6eLFXKZKGTBg8Jc1+fVBzXFzW5sT/ZUCvlFh1aAiVgUXRiYIJ6b/m3cvYdHdC/ht6FwMMbVDgKEhamTlaFAosMIlEAeCZuDtgDEIcPTA8ZIEzB4wFqN6BsDawQkZVfe16U+1S74mN6TGG26+8HX2Rx+rHvgx/RbsDEy1atSOmPyUTPMldQjLiAEvPBNTciSnlWrV1AvnzlUy1cmAAYP/GYWqj4kTJ46EWHjkvKeBFQb540z/qWBpgDJZk3a5vciqRMxzcMGclCycDhiJ9+/fxixCkJ8SUvW4shtJIxdhWexvaFIp0UC2a6nhOD52IfoRIq6Uc2Bu5ghXQqTLo0OwvSQdUYSoB8ddBOpLgMwCDE+sUVnJWUtOHD9+kKlGBgwY/E8TKsXkqVMNOWz2+goD7muR3oaAi5V29ZLkUfNR0ZwKJwMn7MwthVIjR4mkGc1qJV736I8SaQOCrV1ht/+fgFKBD4fOw9qIX/DG4NnYFDgR2WUZgKEVmuTNsBeZoFEpx+CY0yhJjIJlSi0GV8ivCnn8ZcePHcvrZJEeBgwYMPiPg/vvHHwmJISuJ/j6rNmzf5wSV/t1UUbj03GelfBTH6TvW8bkHr3wpns/fJ8dB1eREaJrS5FeX4lniDl/IOcuIU0zQKLLbiKk+V1egpZQxcY2iKkqwt78ezhRQsg1Nw+OaTWYUqWINhSKVh07czZSoVComOpjwIDB30ahtmNmLpc1a84cZ1mzZFUzn/3cJQe+GDYiwMEeUDRhQs9BuFBXgVHGFphv1xMvpd7ATaJIc5rqsDDuHFB8HxAZo5drX9wrIp9LK8jWiOBShcy8UXlBYGiw6fz58zframoUTLUxYMDgb02o+jA3NzcYN2HCcElz82Q1C7NqxBzbCAs+wCeXE7EBAUf7zicYW5LPBkB5LlCn1r5DemCVHKYSda1Yrr7AFQrOxN+5cy4rM7OGqSoGDBj8vyTUh+HXq5eDj4+PAzHTB5M/6errHTk+ZWw2+1pZeXlFZEREFlM1DBgwYMCAAQMGDBgwYMCAAQMGDBgwYMCAAQMGfx/8nwADAGJafmUpUO0HAAAAAElFTkSuQmCC";


        #region Methods

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            const string SEPARATOR = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(SEPARATOR);
                sb.Append(state.Name);
                sb.Append(SEPARATOR);
                sb.Append(state.Abbreviation);
                sb.Append(SEPARATOR);
                sb.Append(state.Published);
                sb.Append(SEPARATOR);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine);  //new line
            }
            return sb.ToString();
        }

        public virtual void ExportReportContributionPaymentToXlsx(Stream stream, Customer customer, Contribution contribution, IList<ReportContributionPayment> reportContributionPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Aportaciones");
                try
                {
                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }

                #region Summary
                worksheet.Cells["A6:A8"].Style.Font.Bold = true;
                worksheet.Cells["D6:D8"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Aportante:";
                worksheet.Cells["B6"].Value = customer.GetFullName();
                worksheet.Cells["A7"].Value = "Dni:";
                worksheet.Cells["B7"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);
                worksheet.Cells["A8"].Value = "N° Adm:";
                worksheet.Cells["B8"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);

                worksheet.Cells["D6"].Value = "Monto:";
                worksheet.Cells["F6"].Value = contribution.AmountPayed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D7"].Value = "Aportante desde:";
                worksheet.Cells["F7"].Value = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, TimeZoneInfo.Utc).ToShortDateString();
                worksheet.Cells["D8"].Value = "Ultimo Pago:";
                if (contribution.UpdatedOnUtc.HasValue)
                    worksheet.Cells["F8"].Value = _dateTimeHelper.ConvertToUserTime(contribution.UpdatedOnUtc.Value, TimeZoneInfo.Utc).ToShortDateString();
                #endregion

                #region Leyend

                worksheet.Cells["M3:M8"].Style.Font.Bold = true;
                worksheet.Cells["L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L3"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.Pendiente)));
                worksheet.Cells["M3"].Value = ContributionState.Pendiente.ToString();
                worksheet.Cells["L4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L4"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.EnProceso)));
                worksheet.Cells["M4"].Value = ContributionState.EnProceso.ToString();
                worksheet.Cells["L5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L5"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.PagoParcial)));
                worksheet.Cells["M5"].Value = ContributionState.PagoParcial.ToString();
                worksheet.Cells["L6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L6"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.Pagado)));
                worksheet.Cells["M6"].Value = ContributionState.Pagado.ToString() + " Automático";
                worksheet.Cells["L7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L7"].Style.Fill.BackgroundColor.SetColor(GetColor(0, ((int)ContributionState.Pagado)));
                worksheet.Cells["M7"].Value = ContributionState.Pagado.ToString() + " Manual";
                worksheet.Cells["L8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L8"].Style.Fill.BackgroundColor.SetColor(GetColor(0, ((int)ContributionState.SinLiquidez)));
                worksheet.Cells["M8"].Value = ContributionState.SinLiquidez.ToString();
                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Año",
                        "Enero","Febrero","Marzo","Abril","Mayo",
                        "Junio","Julio","Agosto","Setiembre","Octubre",
                        "Noviembre","Diciembre", "Total"
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[10, i + 1].Value = properties[i];
                    worksheet.Cells[10, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[10, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[10, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[10, i + 1].Style.Font.Bold = true;
                }

                int row = 11;
                decimal ene, feb, mar, abr, may, jun, jul, ago, sep, oct, nov, dic, total;
                int t;
                foreach (var p in reportContributionPayment)
                {
                    int col = 1;
                    if (worksheet.Cells[row - 1, col].Value != null && int.TryParse(worksheet.Cells[row - 1, col].Value.ToString(), out t)
                        && Convert.ToInt32(worksheet.Cells[row - 1, col].Value.ToString()) == p.Year)
                        row--;

                    worksheet.Cells[row, col].Value = p.Year;
                    col++;

                    ene = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Ene);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = ene;
                    col++;

                    feb = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Feb);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = feb;
                    col++;

                    mar = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Mar);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = mar;
                    col++;

                    abr = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Abr);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = abr;
                    col++;

                    may = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.May);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = may;
                    col++;

                    jun = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Jun);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = jun;
                    col++;

                    jul = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Jul);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = jul;
                    col++;

                    ago = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Ago);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = ago;
                    col++;

                    sep = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Sep);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = sep;
                    col++;

                    oct = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Oct);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = oct;
                    col++;

                    nov = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Nov);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = nov;
                    col++;

                    dic = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Dic);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = dic;
                    col++;

                    total = ene + feb + mar + abr + may + jun + jul + ago + sep + oct + nov + dic;
                    worksheet.Cells[row, col].Value = total;
                    worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                    col++;

                    row++;
                }

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }

                xlPackage.Save();
            }
        }

        public virtual void ExportReportLoanPaymentToXlsx(Stream stream, Customer customer, Loan loan, IList<ReportLoanPayment> reportLoanPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Apoyo Social Económico");
                try
                {
                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }

                #region Summary

                worksheet.Cells["A6:A9"].Style.Font.Bold = true;
                worksheet.Cells["E6:E9"].Style.Font.Bold = true;
                worksheet.Cells["G6:G9"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Aportante:";
                worksheet.Cells["B6:C6"].Merge = true;
                worksheet.Cells["B6"].Value = customer.GetFullName();
                worksheet.Cells["A7"].Value = "Dni:";
                worksheet.Cells["B7:C7"].Merge = true;
                worksheet.Cells["B7"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);
                worksheet.Cells["A8"].Value = "N° Adm:";
                worksheet.Cells["B8:C8"].Merge = true;
                worksheet.Cells["B8"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);
                worksheet.Cells["A9"].Value = "Solicitado:";
                worksheet.Cells["B9:C9"].Merge = true;
                worksheet.Cells["B9"].Value = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                worksheet.Cells["E6"].Value = "Plazo:";
                worksheet.Cells["F6"].Value = string.Format("{0} Meses", loan.Period);
                worksheet.Cells["E7"].Value = "Cuota Mensual:";
                worksheet.Cells["F7"].Value = loan.MonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["E8"].Value = "Importe:";
                worksheet.Cells["F8"].Value = loan.LoanAmount.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["E9"].Value = "Total a Girar:";
                worksheet.Cells["F9"].Value = loan.TotalToPay.ToString("c", new CultureInfo("es-PE"));

                worksheet.Cells["G6"].Value = "T.E.A:";
                worksheet.Cells["H6"].Value = (loan.Tea / 100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G7"].Value = "Seg Desgravamen:";
                worksheet.Cells["H7"].Value = (loan.Safe / 100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G8"].Value = "Total Intereses:";
                worksheet.Cells["H8"].Value = loan.TotalFeed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["G9"].Value = "Total Desgravamen:";
                worksheet.Cells["H9"].Value = loan.TotalSafe.ToString("c", new CultureInfo("es-PE"));

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Cuota","Año","Mes","Capital","Interes","Cuota Mensual","Monto Pagado","Estado"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[11, i + 1].Value = properties[i];
                    worksheet.Cells[11, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[11, i + 1].Style.Font.Bold = true;
                }

                var row = 12;
                var totalMonthlyCapital = 0M;
                var totalMonthlyFee = 0M;
                var totalMonthlyQuota = 0M;
                var totalMonthlyPayed = 0M;

                foreach (var p in reportLoanPayment)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.Quota;
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = p.Year;
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthName;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyCapital.ToString("c", new CultureInfo("es-PE"));
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyFee.ToString("c", new CultureInfo("es-PE"));
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyPayed.ToString("c", new CultureInfo("es-PE"));
                    worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    col++;
                    worksheet.Cells[row, col].Value = GetStateLoanName(p.StateId);

                    totalMonthlyCapital += p.MonthlyCapital;
                    totalMonthlyFee += p.MonthlyFee;
                    totalMonthlyQuota += p.MonthlyQuota;
                    totalMonthlyPayed += p.MonthlyPayed;
                    row++;
                }

                worksheet.Cells[row, 1].Value = "Total";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 4].Value = totalMonthlyCapital.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells[row, 4].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Value = totalMonthlyFee.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells[row, 5].Style.Font.Bold = true;
                worksheet.Cells[row, 6].Value = totalMonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells[row, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells[row, 6].Style.Font.Bold = true;
                worksheet.Cells[row, 7].Value = totalMonthlyPayed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells[row, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells[row, 7].Style.Font.Bold = true;

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportReportLoanPaymentKardexToXlsx(Stream stream, Customer customer, Loan loan, IList<ReportLoanPaymentKardex> reportLoanPaymentKardex)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Apoyo Social Económico Kardex");

                try
                {
                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }

                #region Summary

                worksheet.Cells["A6:A9"].Style.Font.Bold = true;
                worksheet.Cells["D6:D9"].Style.Font.Bold = true;
                worksheet.Cells["G6:G9"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Nombre:";
                worksheet.Cells["B6"].Value = customer.GetFullName();
                worksheet.Cells["A7"].Value = "Dni:";
                worksheet.Cells["B7"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);
                worksheet.Cells["A8"].Value = "N° Adm:";
                worksheet.Cells["B8"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);
                worksheet.Cells["A9"].Value = "Fecha de Solicitud:";
                worksheet.Cells["B9"].Value = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                worksheet.Cells["D6"].Value = "Plazo:";
                worksheet.Cells["E6"].Value = string.Format("{0} Meses", loan.Period);
                worksheet.Cells["D7"].Value = "Cuota Mensual:";
                worksheet.Cells["E7"].Value = loan.MonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D8"].Value = "Importe:";
                worksheet.Cells["E8"].Value = loan.LoanAmount.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D9"].Value = "Total a Girar:";
                worksheet.Cells["E9"].Value = loan.TotalToPay.ToString("c", new CultureInfo("es-PE"));

                worksheet.Cells["G6"].Value = "T.E.A:";
                worksheet.Cells["H6"].Value = (loan.Tea / 100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G7"].Value = "Seg Desgravamen:";
                worksheet.Cells["H7"].Value = (loan.Safe / 100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G8"].Value = "Total Intereses:";
                worksheet.Cells["H8"].Value = loan.TotalFeed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["G9"].Value = "Total Desgravamen:";
                worksheet.Cells["H9"].Value = loan.TotalSafe.ToString("c", new CultureInfo("es-PE"));

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Estado","Tipo","Año","Mes","Monto Pagado",
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[11, i + 1].Value = properties[i];
                    worksheet.Cells[11, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[11, i + 1].Style.Font.Bold = true;
                }

                var row = 12;

                foreach (var p in reportLoanPaymentKardex)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = GetStateLoanName(p.StateId);
                    col++;
                    worksheet.Cells[row, col].Value = p.IsAutomatic == 1 ? "Automático" : "Manual";
                    col++;
                    worksheet.Cells[row, col].Value = p.Year;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthName;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyPayed.ToString("c", new CultureInfo("es-PE"));

                    row++;
                }

                worksheet.Cells[row, 1].Value = "Total Amortizado";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Value = loan.TotalPayed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells[row, 5].Style.Font.Bold = true;

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportReportContributionBenefitToXlsx(Stream stream, Customer customer, ContributionBenefit contributionBenefit,
            IList<ReportContributionBenefit> reportContributionBenefit)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            var report = reportContributionBenefit.FirstOrDefault();

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add(report.BenefitName);
                try
                {
                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }

                #region 1. DATOS GENERALES :

                worksheet.Cells["C5"].Value = report.BenefitType.ToUpper();
                worksheet.Cells["C5"].Style.Font.Bold = true;
                worksheet.Cells["C6"].Value = report.BenefitName.ToUpper();
                worksheet.Cells["C6"].Style.Font.Bold = true;
                worksheet.Cells["C7"].Value = "Nº DE LIQUIDACION: " + report.NumberOfLiquidation;
                worksheet.Cells["C7"].Style.Font.Bold = true;
                worksheet.Cells["A7:D7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;

                worksheet.Cells["A9"].Value = "1. DATOS GENERALES :";
                worksheet.Cells["A9"].Style.Font.Bold = true;
                worksheet.Cells["A9"].Style.Font.UnderLine = true;

                worksheet.Cells["B10"].Value = "1.1";
                worksheet.Cells["B10"].Style.Font.Bold = true;
                worksheet.Cells["C10"].Value = "N° Adm:";
                worksheet.Cells["C10"].Style.Font.Bold = true;
                worksheet.Cells["D10"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);

                worksheet.Cells["B11"].Value = "1.2";
                worksheet.Cells["B11"].Style.Font.Bold = true;
                worksheet.Cells["C11"].Value = "Dni:";
                worksheet.Cells["C11"].Style.Font.Bold = true;
                worksheet.Cells["D11"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);

                worksheet.Cells["B12"].Value = "1.3";
                worksheet.Cells["B12"].Style.Font.Bold = true;
                worksheet.Cells["C12"].Value = "Apellidos y Nombres:";
                worksheet.Cells["C12"].Style.Font.Bold = true;
                worksheet.Cells["D12"].Value = customer.GetFullName();


                worksheet.Cells["B13"].Value = "1.4";
                worksheet.Cells["B13"].Style.Font.Bold = true;
                worksheet.Cells["C13"].Value = "Fecha Ingreso:";
                worksheet.Cells["C13"].Style.Font.Bold = true;
                worksheet.Cells["D13"].Value = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                worksheet.Cells["B14"].Value = "1.5";
                worksheet.Cells["B14"].Style.Font.Bold = true;
                worksheet.Cells["C14"].Value = "Años Aportados:";
                worksheet.Cells["C14"].Style.Font.Bold = true;
                worksheet.Cells["D14"].Value = report.YearInActivity + " años";

                var offset = 15;

                if (!string.IsNullOrEmpty(contributionBenefit.CustomValue1))
                {
                    worksheet.Cells["B" + offset].Value = "1." + (offset - 9);
                    worksheet.Cells["B" + offset].Style.Font.Bold = true;
                    worksheet.Cells["C" + offset].Value = contributionBenefit.CustomField1;
                    worksheet.Cells["C" + offset].Style.Font.Bold = true;
                    worksheet.Cells["D" + offset].Value = contributionBenefit.CustomValue1;
                    offset++;
                }

                if (!string.IsNullOrEmpty(contributionBenefit.CustomValue2))
                {
                    worksheet.Cells["B" + offset].Value = "1." + (offset - 9);
                    worksheet.Cells["B" + offset].Style.Font.Bold = true;
                    worksheet.Cells["C" + offset].Value = contributionBenefit.CustomField2;
                    worksheet.Cells["C" + offset].Style.Font.Bold = true;
                    worksheet.Cells["D" + offset].Value = contributionBenefit.CustomValue2;
                    offset++;
                }

                offset++;
                offset++;

                #endregion

                #region 2. CALCULO AUXILIO ECONOMICO :

                worksheet.Cells["A" + offset].Value = "2. CALCULO AUXILIO ECONOMICO";
                worksheet.Cells["A" + offset].Style.Font.Bold = true;
                worksheet.Cells["A" + offset].Style.Font.UnderLine = true;
                offset++;

                worksheet.Cells["B" + offset].Value = "2.1";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "En Base al Beneficio Economico según Calculo Matematico Actuarial:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.AmountBaseOfBenefit.ToString("c");
                offset++;

                worksheet.Cells["B" + offset].Value = "2.2";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Factor Variable Según Años Aportados:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TabValue;
                offset++;

                worksheet.Cells["B" + offset].Value = "2.3";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Porcentaje a Pagar:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.Discount.ToString("P");
                offset++;

                #region 2.4 Sumatoria Aportes y Apoyo del Fondo de Reserva

                worksheet.Cells["B" + offset].Value = "2.4";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Sumatoria Aportes y Apoyo del Fondo de Reserva:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                offset++;

                worksheet.Cells["C" + offset].Value = "2.4.1 Aportes del Asociado en Actividad:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TotalContributionCopere.ToString("c");
                offset++;

                worksheet.Cells["C" + offset].Value = "2.4.2. Aportes del Asociado en Retiro:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TotalContributionCaja.ToString("c");
                offset++;

                worksheet.Cells["C" + offset].Value = "2.4.3. Aportes Pagos Personales:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TotalContributionPersonalPayment.ToString("c");
                offset++;

                worksheet.Cells["C" + offset].Value = "2.4.4. Apoyo Fondo de Reserva:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.ReserveFund.ToString("c");
                offset++;

                worksheet.Cells["C" + offset].Value = "2.4.5. Aportacion Total:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                worksheet.Cells["D" + offset].Value = report.SubTotalToPay.ToString("c");
                worksheet.Cells["D" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                offset++;

                #endregion

                #region 2.4 Deducciones

                worksheet.Cells["B" + offset].Value = "2.5";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Deducciones";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                offset++;

                worksheet.Cells["C" + offset].Value = "2.5.1. Prestamos Pendites:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TotalLoan;
                offset++;

                worksheet.Cells["C" + offset].Value = "2.5.2. Aportes Pendientes:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = report.TotalLoanToPay.ToString("c");
                offset++;

                worksheet.Cells["C" + offset].Value = "2.5.3. Deducción Total:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                worksheet.Cells["D" + offset].Value = report.TotalLoanToPay.ToString("c");
                worksheet.Cells["D" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                offset++;

                #endregion

                #region 2.6 Beneficio Económico a Liquidar:

                worksheet.Cells["B" + offset].Value = "2.6";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Beneficio Económico a Liquidar:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["D" + offset].Value = (report.TotalToPay).ToString("C", new CultureInfo("es-PE"));
                offset++;

                #endregion

                #region 2.7 Pago Beneficiarios según Sucesion Intestada Nº 1863

                worksheet.Cells["B" + offset].Value = "2.7";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Pago Beneficiarios según Sucesion Intestada Nº 1863";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                offset++;

                var checks = report.Checks.Split('|');
                var index = 1;
                foreach (var check in checks)
                {
                    worksheet.Cells["C" + offset].Value = "2.7." + index + check.Split('-')[0];
                    worksheet.Cells["C" + offset].Style.Font.Bold = true;
                    worksheet.Cells["D" + offset].Value = Convert.ToDecimal(check.Split('-')[1]).ToString("c");
                    offset++;
                    index++;
                }
                #endregion

                worksheet.Cells["B" + offset].Value = "2.8";
                worksheet.Cells["B" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Value = "Auxilio Economico total a pagar:";
                worksheet.Cells["C" + offset].Style.Font.Bold = true;
                worksheet.Cells["C" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["C" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                worksheet.Cells["D" + offset].Value = report.TotalToPay.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D" + offset].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D" + offset].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#77dd77"));
                offset++;
                offset++;
                offset++;
                offset++;
                offset++;

                #endregion

                #region Firmas

                worksheet.Cells["A" + offset].Style.Border.Bottom.Style = ExcelBorderStyle.DashDot;
                worksheet.Cells["A" + (offset + 1)].Value = _signatureSettings.BenefitRightName;
                worksheet.Cells["A" + (offset + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A" + (offset + 2)].Value = _signatureSettings.BenefitRightPosition;
                worksheet.Cells["A" + (offset + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A" + (offset + 3)].Value = _signatureSettings.DefaultName;
                worksheet.Cells["A" + (offset + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                worksheet.Cells["C" + offset].Style.Border.Bottom.Style = ExcelBorderStyle.DashDot;
                worksheet.Cells["C" + (offset + 1)].Value = _signatureSettings.BenefitCenterName;
                worksheet.Cells["C" + (offset + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["C" + (offset + 2)].Value = _signatureSettings.BenefitCenterName;
                worksheet.Cells["C" + (offset + 2)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["C" + (offset + 3)].Value = _signatureSettings.DefaultName;
                worksheet.Cells["C" + (offset + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;

                worksheet.Cells["E" + (offset)].Style.Border.Bottom.Style = ExcelBorderStyle.DashDot;
                worksheet.Cells["E" + (offset + 1)].Value = _signatureSettings.BenefitLeftName;
                worksheet.Cells["E" + (offset + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["E" + (offset + 2)].Value = _signatureSettings.BenefitLeftPosition;
                worksheet.Cells["E" + (offset + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["E" + (offset + 3)].Value = _signatureSettings.DefaultName;
                worksheet.Cells["E" + (offset + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                #endregion

                var imagePathSignature = _webHelper.MapPath("/Administration/Content/images/Escudo.png");
                var imageSignature = new Bitmap(imagePathSignature);
                var excelImageSignature = worksheet.Drawings.AddPicture("Firma", imageSignature);
                excelImageSignature.From.Column = 2;
                excelImageSignature.From.Row = offset + 4;
                excelImageSignature.SetSize(115, 115);
                excelImageSignature.AdjustPositionAndSize();

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }

        }

        public virtual void ExportGlobalReportToXlsx(MemoryStream stream, int year, int month, IList<ReportGlobal> globalReport)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Resumen");
                // var imagePath = _webHelper.MapPath(@"C:\inetpub\wwwroot\Acmr\Administration\Content\images\logo.png");
                try
                {

                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }


                #region Summary

                var date = new DateTime(year, month, 1);

                worksheet.Cells["C5:C7"].Style.Font.Bold = true;
                worksheet.Cells["C5:C7"].Style.Font.Size = 20;
                worksheet.Cells["C5:C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["C5"].Value = "APORTACIONES / APOYO SOCIAL ECONOMICO";
                worksheet.Cells["C6"].Value = "COPERE / CAJA / OTROS";
                worksheet.Cells["C7"].Value = "MES: " + date.ToString("MMMM", new CultureInfo("es-PE")).ToUpper() + " AÑO: " + year;

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Tipo","N° Administrativo","Asociado","Número","Valor de Cuota","Proceso","Entidad","Estado"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[9, i + 1].Value = properties[i];
                    worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[9, i + 1].Style.Font.Bold = true;
                }

                var row = 10;
                var total = 0M;
                foreach (var p in globalReport)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.Source;
                    col++;
                    worksheet.Cells[row, col].Value = p.AdmCode;
                    col++;
                    worksheet.Cells[row, col].Value = p.LastName + " ,  " + p.FirstName;
                    col++;
                    worksheet.Cells[row, col].Value = p.Number;
                    col++;
                    worksheet.Cells[row, col].Value = p.Payed.ToString("c", new CultureInfo("es-PE"));
                    col++;
                    worksheet.Cells[row, col].Value = p.IsAutomatic == 1 ? "Automático" : "Manual";
                    col++;
                    worksheet.Cells[row, col].Value = p.BankName;
                    col++;
                    worksheet.Cells[row, col].Value = p.Source == "Aportaciones" ? GetStateContributionName(p.StateId) : GetStateLoanName(p.StateId);

                    total += p.Payed;
                    row++;
                }

                worksheet.Cells[row, 4].Value = "Total";
                worksheet.Cells[row, 4].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Value = total.ToString("c", new CultureInfo("es-PE"));

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportDetailLoanToXlsx(MemoryStream stream, DateTime from, DateTime to, string source, IList<ReportLoanDetail> reportLoan)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Apoyo Social Económico");
                // var imagePath = _webHelper.MapPath(@"C:\inetpub\wwwroot\Acmr\Administration\Content\images\logo.png");
                try
                {

                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e) { }


                #region Summary
                worksheet.Cells["E5:K5"].Merge = true;
                worksheet.Cells["E6:K6"].Merge = true;
                worksheet.Cells["E7:K7"].Merge = true;
                worksheet.Cells["E8:K8"].Merge = true;
                worksheet.Cells["E5:K8"].Style.Font.Bold = true;
                worksheet.Cells["E5:K8"].Style.Font.Size = 20;
                worksheet.Cells["E5:K8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["E5"].Value = "APOYO SOCIAL ECONOMICO";
                worksheet.Cells["E6"].Value = "REPORTE";
                worksheet.Cells["E7"].Value = source.ToUpper();
                worksheet.Cells["E8"].Value = "DESDE :" + from.ToShortDateString() + " - HASTA:" + to.ToShortDateString();

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Fecha de Giro","Estado de Apoyo","N° de orden","N° Cheque","Estado Asociado","Fuente","Asociado","N° Administrativo","Importe Solicitado","Gravamen","Importe Girado",
                        "Interes Total","Total Deuda (Capital+Interes)", "Total pagado a la fecha","Capital de lo pagado","interes de lo pagado",
                        "Total saldo a la fecha","N° Cotas","Cuota Mensual","Capital de la Cuota","Interes de la couta","Ultimo Pago"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[9, i + 1].Value = properties[i];
                    worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[9, i + 1].Style.Font.Bold = true;
                }

                var row = 10;
                var totalLoanAmount = 0M;
                var totalTotalSafe = 0M;
                var totalTotalToPay = 0M;
                var totalTotalFeed = 0M;
                var totalTotalAmount = 0M;
                var totalTotalPayed = 0M;
                var totalPayedCapital = 0M;
                var totalPayedTax = 0M;
                var totalDebit = 0M;
                var totalMonthlyQuota = 0M;
                var totalQoutaCapital = 0M;
                var totalQoutaTax = 0M;
                foreach (var p in reportLoan)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.ApprovalDate;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanState;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanNumber;
                    col++;
                    worksheet.Cells[row, col].Value = p.CheckNumber;
                    col++;
                    worksheet.Cells[row, col].Value = p.CustomerState;
                    col++;
                    worksheet.Cells[row, col].Value = p.Source;
                    col++;
                    worksheet.Cells[row, col].Value = p.LastName + " ,  " + p.FirstName;
                    col++;
                    worksheet.Cells[row, col].Value = p.AdmCode;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanAmount.ToString("c", new CultureInfo("es-PE"));
                    totalLoanAmount += p.LoanAmount;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalSafe.ToString("c", new CultureInfo("es-PE"));
                    totalTotalSafe += p.TotalSafe;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalToPay.ToString("c", new CultureInfo("es-PE"));
                    totalTotalToPay += p.TotalToPay;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalFeed.ToString("c", new CultureInfo("es-PE"));
                    totalTotalFeed += p.TotalFeed;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalAmount.ToString("c", new CultureInfo("es-PE"));
                    totalTotalAmount += p.TotalAmount;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalPayed.ToString("c", new CultureInfo("es-PE"));
                    totalTotalPayed += p.TotalPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.PayedCapital.ToString("c", new CultureInfo("es-PE"));
                    totalPayedCapital += p.PayedCapital;
                    col++;
                    worksheet.Cells[row, col].Value = p.PayedTax.ToString("c", new CultureInfo("es-PE"));
                    totalPayedTax += p.PayedTax;
                    col++;
                    worksheet.Cells[row, col].Value = p.Debit.ToString("c", new CultureInfo("es-PE"));
                    totalDebit += p.Debit;
                    col++;
                    worksheet.Cells[row, col].Value = p.Period;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                    totalMonthlyQuota += p.MonthlyQuota;
                    col++;
                    worksheet.Cells[row, col].Value = p.QoutaCapital.ToString("c", new CultureInfo("es-PE"));
                    totalQoutaCapital += p.QoutaCapital;
                    col++;
                    worksheet.Cells[row, col].Value = p.QoutaTax.ToString("c", new CultureInfo("es-PE"));
                    totalQoutaTax += p.QoutaTax;
                    col++;
                    worksheet.Cells[row, col].Value = p.LastDate;
                    col++;

                    row++;
                }

                worksheet.Cells[row, 9].Style.Font.Bold = true;
                worksheet.Cells[row, 9].Value = totalLoanAmount;
                worksheet.Cells[row, 10].Style.Font.Bold = true;
                worksheet.Cells[row, 10].Value = totalTotalSafe;
                worksheet.Cells[row, 11].Style.Font.Bold = true;
                worksheet.Cells[row, 11].Value = totalTotalToPay;
                worksheet.Cells[row, 12].Style.Font.Bold = true;
                worksheet.Cells[row, 12].Value = totalTotalFeed;
                worksheet.Cells[row, 13].Style.Font.Bold = true;
                worksheet.Cells[row, 13].Value = totalTotalAmount;
                worksheet.Cells[row, 14].Style.Font.Bold = true;
                worksheet.Cells[row, 14].Value = totalTotalPayed;
                worksheet.Cells[row, 15].Style.Font.Bold = true;
                worksheet.Cells[row, 15].Value = totalPayedCapital;
                worksheet.Cells[row, 16].Style.Font.Bold = true;
                worksheet.Cells[row, 16].Value = totalPayedTax;
                worksheet.Cells[row, 17].Style.Font.Bold = true;
                worksheet.Cells[row, 17].Value = totalDebit;
                worksheet.Cells[row, 19].Style.Font.Bold = true;
                worksheet.Cells[row, 19].Value = totalMonthlyQuota;
                worksheet.Cells[row, 20].Style.Font.Bold = true;
                worksheet.Cells[row, 20].Value = totalQoutaCapital;
                worksheet.Cells[row, 21].Style.Font.Bold = true;
                worksheet.Cells[row, 21].Value = totalQoutaTax;

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportSummaryContributionToXlsx(MemoryStream stream, int fromId, int toId, int typeId, IList<ReportSummaryContribution> summaryContribution)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Aportacines");
                // var imagePath = _webHelper.MapPath(@"C:\inetpub\wwwroot\Acmr\Administration\Content\images\logo.png");
                try
                {

                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e)
                {
                }

                #region Summary
                worksheet.Cells["A5:O5"].Merge = true;
                worksheet.Cells["A6:O6"].Merge = true;
                worksheet.Cells["A5:O6"].Style.Font.Bold = true;
                worksheet.Cells["A5:O6"].Style.Font.Size = 20;
                worksheet.Cells["A5:O6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A5"].Value = "CONSOLIDADO DE APORTACIONES";
                worksheet.Cells["A6"].Value = "ENERO " + fromId + " DICIEMBRE " + toId;

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "N° Administrativo","Asociado","Fuente",
                        "Enero","Febrero","Marzo","Abril","Mayo",
                        "Junio","Julio","Agosto","Setiembre","Octubre",
                        "Noviembre","Diciembre","Total"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[9, i + 1].Value = properties[i];
                    worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[9, i + 1].Style.Font.Bold = true;
                }

                var row = 10;
                var totalEne = 0M;
                var totalFeb = 0M;
                var totalMar = 0M;
                var totalAbr = 0M;
                var totalMay = 0M;
                var totalJun = 0M;
                var totalJul = 0M;
                var totalAgo = 0M;
                var totalSep = 0M;
                var totalOct = 0M;
                var totalNov = 0M;
                var totalDic = 0M;
                var totalLine = 0M;

                foreach (var p in summaryContribution)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.CustomerAdmCode;
                    col++;
                    worksheet.Cells[row, col].Value = p.CustomerLastName + " ," + p.CustomerName;
                    col++;
                    worksheet.Cells[row, col].Value = p.TypeSource;
                    col++;
                    worksheet.Cells[row, col].Value = p.Ene;
                    totalEne += p.Ene;
                    col++;
                    worksheet.Cells[row, col].Value = p.Feb;
                    totalFeb += p.Feb;
                    col++;
                    worksheet.Cells[row, col].Value = p.Mar;
                    totalMar += p.Mar;
                    col++;
                    worksheet.Cells[row, col].Value = p.Abr;
                    totalAbr += p.Abr;
                    col++;
                    worksheet.Cells[row, col].Value = p.May;
                    totalMay += p.May;
                    col++;
                    worksheet.Cells[row, col].Value = p.Jun;
                    totalJun += p.Jun;
                    col++;
                    worksheet.Cells[row, col].Value = p.Jul;
                    totalJul += p.Jul;
                    col++;
                    worksheet.Cells[row, col].Value = p.Ago;
                    totalAgo += p.Ago;
                    col++;
                    worksheet.Cells[row, col].Value = p.Sep;
                    totalSep += p.Sep;
                    col++;
                    worksheet.Cells[row, col].Value = p.Oct;
                    totalOct += p.Oct;
                    col++;
                    worksheet.Cells[row, col].Value = p.Nov;
                    totalNov += p.Nov;
                    col++;
                    worksheet.Cells[row, col].Value = p.Dic;
                    totalDic += p.Dic;
                    col++;
                    worksheet.Cells[row, col].Value = p.Ene + p.Feb + p.Mar + p.Abr + p.May + p.Jul + p.Jun + p.Ago + p.Sep + p.Oct + p.Nov + p.Dic;
                    totalLine += p.Ene + p.Feb + p.Mar + p.Abr + p.May + p.Jul + p.Jun + p.Ago + p.Sep + p.Oct + p.Nov + p.Dic;
                    col++;
                    row++;
                }

                worksheet.Cells[row, 4].Value = totalEne;
                worksheet.Cells[row, 5].Value = totalFeb;
                worksheet.Cells[row, 6].Value = totalMar;
                worksheet.Cells[row, 7].Value = totalAbr;
                worksheet.Cells[row, 8].Value = totalMay;
                worksheet.Cells[row, 9].Value = totalJun;
                worksheet.Cells[row, 10].Value = totalJul;
                worksheet.Cells[row, 11].Value = totalAgo;
                worksheet.Cells[row, 12].Value = totalSep;
                worksheet.Cells[row, 13].Value = totalOct;
                worksheet.Cells[row, 14].Value = totalNov;
                worksheet.Cells[row, 15].Value = totalDic;
                worksheet.Cells[row, 16].Value = totalLine;


                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportMilitarSituationToXlsx(MemoryStream stream, string militarySituation, IList<ReportMilitarSituation> militarSituations)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Situacion Militar");
                // var imagePath = _webHelper.MapPath(@"C:\inetpub\wwwroot\Acmr\Administration\Content\images\logo.png");
                try
                {

                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e)
                {
                }

                #region Summary
                worksheet.Cells["A5:M5"].Merge = true;
                worksheet.Cells["A6:M6"].Merge = true;
                worksheet.Cells["A5:M6"].Style.Font.Bold = true;
                worksheet.Cells["A5:M6"].Style.Font.Size = 20;
                worksheet.Cells["A5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A5"].Value = "SITUACION MILITAR DE LOS APORTANTES";
                worksheet.Cells["A6"].Value = militarySituation.ToUpper();

                #endregion

                var properties = new[]
                    {
                        "N° Administrativo","Asociado","Situacion Militar",
                        "Estado Aportacion","Autorizacion Descuento","Monto por Aportar","Monto Abonado (a la fecha)","Monto Aportacion Pendiente",
                        "Estado Apoyo","N° Orden de Prestamo","Monto Solicitado", "Monto del Apoyo", "Monto Apoyo Pendiente",
                        "Monto Pagado (a la fecha)","Perido"
                        
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[9, i + 1].Value = properties[i];
                    worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[9, i + 1].Style.Font.Bold = true;
                }

                var row = 10;

                foreach (var p in militarSituations)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.AdmCode;
                    col++;
                    worksheet.Cells[row, col].Value = p.LastName + " ," + p.FirstName;
                    col++;
                    worksheet.Cells[row, col].Value = militarySituation;
                    col++;
                    worksheet.Cells[row, col].Value = p.ContributionState ? "Activo" : "Inactivo";
                    col++;
                    worksheet.Cells[row, col].Value = p.ContributionAuthorizeDiscont;
                    col++;
                    worksheet.Cells[row, col].Value = p.ContributionAmountMeta;
                    col++;
                    worksheet.Cells[row, col].Value = p.ContributionAmountPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.ContributionAmountMeta - p.ContributionAmountPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanState ? "Activo" : "Inactivo";
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanNumber;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanAmount;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanTotalAmount;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanTotalAmount - p.LoanTotalPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanTotalPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.LoanPeriod;
                    col++;
                    row++;

                }

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportBenefitToXlsx(MemoryStream stream, Benefit getBenefitById, IList<ReportBenefit> benefit)
        {
            throw new NotImplementedException();
        }

        public virtual string ExportScheduleTxt(ScheduleBatch schedule)
        {
            var nameFile = "";
            if (schedule.SystemName.Trim().ToUpper() == ("KS.BATCH.CAJA.OUT"))
                nameFile = string.Format("6008_{0}00.txt", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));
            else
                nameFile = string.Format("8001_{0}00.txt", schedule.PeriodYear.ToString("0000") + schedule.PeriodMonth.ToString("00"));


            var fileReaded = File.ReadAllLines(Path.Combine(Path.Combine(schedule.PathBase, schedule.FolderMoveToDone),
                        nameFile));

            var sb = new StringBuilder();

            foreach (var s in fileReaded)
            {
                sb.AppendLine(s);
            }

            return sb.ToString();

        }

        public virtual void ExportBankPaymentToXlsx(MemoryStream stream, DateTime from, DateTime to, IList<ReportBankPayment> summaryBankPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Depositos Bancarios");
                // var imagePath = _webHelper.MapPath(@"C:\inetpub\wwwroot\Acmr\Administration\Content\images\logo.png");
                try
                {

                    var image = new Bitmap(new MemoryStream(Convert.FromBase64String(IMAGE)));
                    var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                    excelImage.From.Column = 0;
                    excelImage.From.Row = 0;
                }
                catch (Exception e)
                {
                }

                #region Summary

                worksheet.Cells["A5:M5"].Merge = true;
                worksheet.Cells["A6:M6"].Merge = true;
                worksheet.Cells["A5:M6"].Style.Font.Bold = true;
                worksheet.Cells["A5:M6"].Style.Font.Size = 20;
                worksheet.Cells["A5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells["A5"].Value = "DEPOSITOS BANCARIOS ";
                worksheet.Cells["A6"].Value = "Desde: " + from.ToShortDateString() + " - Hasta: " + to.ToShortDateString();

                #endregion


                var properties = new[]
                {
                    "Nombre", "Cod Adm", "Dni", "Situacion Militar", "Nro Transaccion", "Fecha", "Monto", "Detalle", "Banco"
                };

                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[9, i + 1].Value = properties[i];
                    worksheet.Cells[9, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[9, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[9, i + 1].Style.Font.Bold = true;

                    var row = 10;

                    foreach (var p in summaryBankPayment)
                    {
                        var col = 1;
                        worksheet.Cells[row, col].Value = p.LastName + "," + p.FirstName;
                        col++;
                        worksheet.Cells[row, col].Value = p.AdmCode;
                        col++;
                        worksheet.Cells[row, col].Value = p.Dni;
                        col++;
                        worksheet.Cells[row, col].Value = p.MilitarySituation;
                        col++;
                        worksheet.Cells[row, col].Value = p.TransactionNumber;
                        col++;
                        worksheet.Cells[row, col].Value = p.ProcessedDateOnUtc.ToShortDateString();
                        col++;
                        worksheet.Cells[row, col].Value = p.AmountPayed;
                        col++;
                        worksheet.Cells[row, col].Value = p.Reference;
                        col++;
                        worksheet.Cells[row, col].Value = p.BankName;
                        col++;
                        row++;
                    }
                }

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        public virtual void ExportReportInfoToXlsx(MemoryStream stream, string source, List<Info> info)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add(source.Replace(".", " ").Replace("Ks", "").Replace("Batch", "").Trim());

                var properties = new[]
                    {
                        "Año","Mes","AsociadoId","Nombre","Cod Adm","Dni","Total Aportacion","Total Pagado","Total Apoyo",
                        "Couta Aportacion","Monto 1", "Monto 2", "Monto 3","Monto Anterior","Monto Total","Monto Aportado","EstadoAportacionId","Es Automatico","Banco",
                        "Cuenta","Transaccion","Referencia", "Descripcion",
                        "Couta Apoyo","Couta Mensual","Interes","Capital","Monto Pagado","EstadoApoyoId","Es Automatico","Banco",
                        "Cuenta","Transaccion","Referencia", "Descripcion"
                        
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i];
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                var row = 2;

                foreach (var p in info)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.Year;
                    col++;
                    worksheet.Cells[row, col].Value = p.Month;
                    col++;
                    worksheet.Cells[row, col].Value = p.CustomerId;
                    col++;
                    worksheet.Cells[row, col].Value = p.CompleteName;
                    col++;
                    worksheet.Cells[row, col].Value = p.AdminCode;
                    col++;
                    worksheet.Cells[row, col].Value = p.Dni;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalContribution;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalPayed;
                    col++;
                    worksheet.Cells[row, col].Value = p.TotalLoan;
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Number.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Amount1.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Amount2.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Amount3.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.AmountOld.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.AmountTotal.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.AmountPayed.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.StateId.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.IsAutomatic.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.BankName.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.AccountNumber.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.TransactionNumber.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Reference.ToString() : "";
                    col++;
                    worksheet.Cells[row, col].Value = p.InfoContribution != null ? p.InfoContribution.Description.ToString() : "";
                    col++;

                    foreach (var infoLoan in p.InfoLoans)
                    {
                        worksheet.Cells[row, col].Value = infoLoan.Quota;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.MonthlyQuota;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.MonthlyFee;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.MonthlyCapital;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.MonthlyPayed;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.StateId;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.IsAutomatic;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.BankName;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.AccountNumber;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.TransactionNumber;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.Reference;
                        col++;
                        worksheet.Cells[row, col].Value = infoLoan.Description;
                        col++;
                        row++;
                        col = col - 12;
                    }
                    row++;
                }

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }
                xlPackage.Save();
            }
        }

        #endregion

        #region Utilities

        private Color GetColor(int isAutomatic, int stateId)
        {
            //ContributionState.Pendiente => Color.Gainsboro
            //ContributionState.EnProceso => Color.LightBlue
            //ContributionState.PagoParcial => Color.PaleGreen
            //ContributionState.Pagado.ToString() + " Automático"  => Color.LightPink
            //ContributionState.EnProceso.ToString()+ " Manual"  => Color.PaleGoldenrod

            //#D5D5D5(1)=Pendiente
            //#FFFFd8(2)=En_Proceso
            //#FFDAB4(3)=Pago_Parcial
            //#BDD7EE(4)=Pagado_Automatico
            //#DAFFB4(5)=Pago_Personal
            //#FFB4B4(6)=Sin_Liquidez

            if ((stateId == (int)ContributionState.Pendiente))
                return ColorTranslator.FromHtml("#D5D5D5");
            if (stateId == (int)ContributionState.SinLiquidez)
                return ColorTranslator.FromHtml("#FFB4B4");
            if ((stateId == (int)ContributionState.EnProceso))
                return ColorTranslator.FromHtml("#FFFFd8");
            if ((stateId == (int)ContributionState.PagoParcial))
                return ColorTranslator.FromHtml("#FFDAB4");
            if ((stateId == (int)ContributionState.Pagado) && isAutomatic == 1)
                return ColorTranslator.FromHtml("#BDD7EE");
            if ((stateId == (int)ContributionState.Pagado) && isAutomatic == 0)
                return ColorTranslator.FromHtml("#DAFFB4");

            return ColorTranslator.FromHtml("#D5D5D5");
        }

        private string GetStateContributionName(int stateId)
        {
            switch (stateId)
            {
                case (int)ContributionState.EnProceso: return "En Proceso";
                case (int)ContributionState.Pendiente: return "Pendiente";
                case (int)ContributionState.PagoParcial: return "Pago Parcial";
                case (int)ContributionState.Pagado: return "Pagado";
                case (int)ContributionState.SinLiquidez: return "Sin Liquidez";
                case (int)ContributionState.Devolucion: return "Devolucion";
                case (int)ContributionState.PagoPersonal: return "Pago Personal";
            }
            return "";
        }
        private string GetStateLoanName(int stateId)
        {
            switch (stateId)
            {
                case (int)LoanState.EnProceso: return "En Proceso";
                case (int)LoanState.Pendiente: return "Pendiente";
                case (int)LoanState.PagoParcial: return "Pago Parcial";
                case (int)LoanState.Pagado: return "Pagado";
                case (int)LoanState.SinLiquidez: return "Sin Liquidez";
                case (int)LoanState.Devolucion: return "Devolucion";
                case (int)LoanState.Anulado: return "Anulado";
                case (int)LoanState.PagoPersonal: return "Pago Personal";
                case (int)LoanState.Cancelado: return "Cancelado";
            }
            return "";
        }

        #endregion
    }
}