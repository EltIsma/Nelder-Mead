package main

import (
	"fmt"
	"math"
	"sort"
)

const (
	defaultMaxIterations = 10000
	// reflection coefficient
	defaultAlpha = 1.0
	// contraction coefficient
	defaultBeta = 0.5
	// shrink coefficient
	defaultDelta = 0.5
	// expansion coefficient
	defaultGamma = 2.0
)

// Optimizer represents the parameters to the Nelder-Mead simplex method.
type Optimizer struct {
	// Maximum number of iterations.
	MaxIterations int
	// Reflection coefficient.
	Alpha,
	// Contraction coefficient.
	Beta,
	// Shrink coefficient.
	Delta,
	// Expansion coefficient.
	Gamma float64
}

// New returns a new instance of Optimizer with all values set to the defaults.
func New() *Optimizer {
	return &Optimizer{
		MaxIterations: defaultMaxIterations,
		Alpha:         defaultAlpha,
		Beta:          defaultBeta,
		Delta:         defaultDelta,
		Gamma:         defaultGamma,
	}
}

func (o *Optimizer) Optimize(
	objfunc func([]float64) float64,
	start [][]float64,
	epsilon float64,
	// scale float64,
) (float64, []float64){
	n := len(start)
	values := make([]float64, n)
	centroid := make([]float64, n-1) //mid
	ref := make([]float64, n-1)      //reflection
	exp := make([]float64, n-1)      //expansion
	contr := make([]float64, n-1)    //contraction
	for itr := 1; itr <= o.MaxIterations; itr++ {
		for i := 0; i < n; i++ {
			values[i] = objfunc(start[i])
		}
		sort.Sort(nmVertexSorter{start, values})
		//fmt.Println(values[0], values[1], values[2])
		for i := 0; i < len(start[0]); i++ {
			cent := 0.0
			for m := 0; m < n-1; m++ {

				cent += start[m][i]
			}
			centroid[i] = cent / float64(n-1)
		}

		//reflect the largest value to new vertex ref
		for i := 0; i < n-1; i++ {
			ref[i] = centroid[i] + o.Alpha*(centroid[i]-start[len(start)-1][i])
		}

		fr := objfunc(ref)
		if fr < values[len(values)-2] && fr >= values[0] {
			//start[len(start)-1] = ref
			for i := 0; i < n-1; i++ {
				start[len(start)-1][i] = ref[i]
			}
			values[len(start)-1] = fr
		}

		//expansion
		if fr < values[0] {
			for i := 0; i < n-1; i++ {
				exp[i] = centroid[i] + o.Gamma*(ref[i]-centroid[i])
			}
			fe := objfunc(exp)
			if fe < fr {
				for i := 0; i < n-1; i++ {
					start[len(start)-1][i] = exp[i]
				}
				values[len(start)-1] = fe
			} else {
				for i := 0; i < n-1; i++ {
					start[len(start)-1][i] = ref[i]
				}
				values[len(start)-1] = fr
			}
		}
		//contraction
		if fr >= values[len(values)-2] {
			if fr < values[len(start)-1] {
				// perform outside contraction
				for i := 0; i < n-1; i++ {
					contr[i] = centroid[i] + o.Beta*(ref[i]-centroid[i])
				}

			} else {
				// perform inside contraction
				for i := 0; i < n-1; i++ {
					contr[i] = centroid[i] - o.Beta*(centroid[i]-start[len(start)-1][i])
				}
			}

			fc := objfunc(contr)
			if fc < values[len(start)-1] {
				for i := 0; i < n-1; i++ {
					start[len(start)-1][i] = contr[i]
				}
				values[len(start)-1] = fc
			} else {
				//shrink
				for j := 1; j < n; j++ {
					for i := 0; i < len(start[0]); i++ {
						start[j][i] = start[0][i] + o.Delta*(start[j][i]-start[0][i])
					}
				}

			}

		}
		// вычисляем среднеквадратичное  отклонение
		fsum := 0.0
		for i := 0; i < n; i++ {
			fsum += values[i]
		}
		favg := fsum / float64(n+1)
		s := 0.0
		for i := 0; i < n; i++ {
			s += math.Pow((values[i]-favg), 2.0) / float64(n-1)
		}
		s = math.Sqrt(s)
		if s < epsilon {
			//fmt.Println(itr)
			break
		}

	}
	sort.Sort(nmVertexSorter{start, values})

	return values[0], start[0]

}

type nmVertexSorter struct {
	vertices [][]float64
	values   []float64
}

func (n nmVertexSorter) Len() int {
	return len(n.values)
}

func (n nmVertexSorter) Less(i, j int) bool {
	return n.values[i] < n.values[j]
}

func (n nmVertexSorter) Swap(i, j int) {
	n.values[i], n.values[j] = n.values[j], n.values[i]
	n.vertices[i], n.vertices[j] = n.vertices[j], n.vertices[i]
}

func main() {
	f := func(x []float64) float64 {
		X1, X2:= x[0], x[1]
		return 100* math.Pow(X2 - math.Pow(X1,2), 2) + math.Pow((1-X1),2)
		//X1*X1
		//100* math.Pow(X2 - math.Pow(X1,2), 2) + math.Pow((1-X1),2)
		//X1*X1 + X1*X2 + X2*X2 - 6*X1 - 9*X2
	}
	start := [][]float64{[]float64{9876, -9875}, []float64{7878, -9}, []float64{3, 1}}
	res, coord :=New().Optimize(f, start, 1e-8)
	fmt.Printf("%.3f %.3f",res, coord)
}
