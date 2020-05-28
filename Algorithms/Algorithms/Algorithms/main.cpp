//
//  main.cpp
//  Algorithms
//
//  Created by Vanush Grigoryan on 5/17/20.
//  Copyright © 2020 Vanush Grigoryan. All rights reserved.
//

#include <iostream>
#include <cmath>
#include <array>
#include <unordered_map>
#include <vector>
#include <tuple>
#include <random>
#include <chrono>

enum class WindingOrder: int {
    Clockwise = -1,
    Counterclockwise = 1,
};

struct Vec {
    float x;
    float y;
    
    float len() {
        return sqrt(sqLen());
    }
    
    float sqLen() {
        return x * x + y * y;
    }

};

Vec operator- (const Vec& lhs, const Vec& rhs) {
    return Vec {lhs.x - rhs.x, lhs.y - rhs.y};
}

float dot(const Vec& lhs, const Vec& rhs) {
    return lhs.x * rhs.x + lhs.y * rhs.y;
}

enum class CyclicIteratorDirection {
    Forward,
    Backward
};

template <typename CT>
class CyclicIterator {
public:
    
    CyclicIterator(const CT* values, std::size_t index = 0)
    : _values {values}
    , _index {index} {
        assert(values->size() > 0);
    }
    
    const Vec* operator->() const {
        return &((*_values)[_index]);
    }
    
    const Vec& operator*() const {
        return (*_values)[_index];
    }
    
    CyclicIterator& operator++() {
        _index = (_index + 1) % _values->size();
        return *this;
    }
    
    CyclicIterator operator++(int) {
        CyclicIterator tmp(*this);
        operator++();
        return tmp;
    }
    
    CyclicIterator& operator--() {
        if(_index == 0) {
            _index = _values->size() - 1;
        } else {
            --_index;
        }
        return *this;
    }
    
    CyclicIterator operator--(int) {
        CyclicIterator tmp(*this);
        operator--();
        return tmp;
    }
    
    template <typename T>
    friend bool operator==(const CyclicIterator<T>& lhs, const CyclicIterator<T>& rhs);
    
    template <typename T>
    friend CyclicIterator<T> operator+ (const CyclicIterator<T>& it, int distance);
    
    template <typename T>
    friend int distance(const CyclicIterator<T>& it1, const CyclicIterator<T>& it2, CyclicIteratorDirection direction);
    
private:
    const CT* _values;
    std::size_t _index;
};

template <typename CT>
inline bool operator==(const CyclicIterator<CT>& lhs, const CyclicIterator<CT>& rhs) {
    return lhs._index == rhs._index;
}

template <typename CT>
inline bool operator!=(const CyclicIterator<CT>& lhs, const CyclicIterator<CT>& rhs) {
    return !(lhs == rhs);
}

template <typename CT>
inline CyclicIterator<CT> operator+ (const CyclicIterator<CT>& it, int distance) {
    auto d = (static_cast<int>(it._index) + distance) % static_cast<int>(it._values->size());
    size_t index = (d >= 0 ? d : d + static_cast<int>(it._values->size()));
    return CyclicIterator<CT> {it._values, index};
}

template <typename CT>
inline int distance(const CyclicIterator<CT>& it1, const CyclicIterator<CT>& it2, CyclicIteratorDirection direction) {
    assert(it1._values == it2._values);
    switch (direction) {
        case CyclicIteratorDirection::Forward: {
            if(it1._index <= it2._index) {
                return static_cast<int>(it2._index - it1._index);
            } else {
                return static_cast<int>(it1._values->size() - (it1._index - it2._index));
            }
            break;
        }
        case CyclicIteratorDirection::Backward: {
            if(it1._index <= it2._index) {
                return -static_cast<int>(it1._values->size() - (it2._index - it1._index));
            } else {
                return -static_cast<int>(it1._index - it2._index);
            }
            break;
        }
    }
    assert(false);
}

template <size_t N>
struct Polygon {
    using PointCollection = std::array<Vec, N>;
    PointCollection points;
    
    using CyclicPointIterator = CyclicIterator<PointCollection>;
    
    CyclicIterator<PointCollection> cyclicPointIterator(std::size_t index = 0) const {
        return CyclicIterator {&points, index};
    }
    
};


// Half plane intersection algorithm O(n)
// Assumes that the polygon is convex and points are in counterclockwise order
template <size_t N>
std::vector<bool> isInside_O_N(const Polygon<N>& polygon, const std::vector<Vec>& points) {
    
    std::vector<bool> result;
    result.reserve(points.size());
    for (const auto& point: points) {
        bool isInside = true;
        for(size_t i = 0; i < N; ++i) {
            auto vec = polygon.points[(i + 1) % N] - polygon.points[i];
            auto orthVec = Vec {-vec.y, vec.x};
            auto pVec = point - polygon.points[i];
            if(dot(orthVec, pVec) <= 0.f) {
                isInside = false;
                break;
            }
        }
        result.emplace_back(isInside);
    }
    
    return result;
    
}

template <size_t N>
std::tuple<Vec, Vec, bool> findIntersectionSegment(const Polygon<N>& polygon, typename Polygon<N>::CyclicPointIterator tpi, typename Polygon<N>::CyclicPointIterator bpi, float lineY, WindingOrder wo) {
    
    static_assert(N > 2);
    
    if(lineY > tpi->y || lineY < bpi->y) {
        return std::make_tuple(Vec {}, Vec {}, false);
    }
    
    /*bool found = false;
    auto it = tpi;
    while(it != bpi) {
        auto y = (wo == WindingOrder::Counterclockwise ? ++it : --it)->y;
        if(lineY >= y) {
            found = true;
            break;
        }
    }
    
    assert(found);
    auto bp = *it;
    auto tp = *(wo == WindingOrder::Counterclockwise ? --it : ++it);
    return std::make_tuple(tp, bp, true);
    */
    
    auto dir = (wo == WindingOrder::Counterclockwise ? CyclicIteratorDirection::Forward : CyclicIteratorDirection::Backward);
    
    while(true) {
        
        auto dist = distance(tpi, bpi, dir);
        if(abs(dist) == 1) {
            break;
        }
        
        auto it = tpi + 0.5 * dist;
        if(lineY > it->y) {
            bpi = it;
        } else {
            tpi = it;
        }
    }

    return std::make_tuple(*tpi, *bpi, true);
    
}

float computePointXOnLine(Vec p1, Vec p2, float py) {
    
    // (X - P) • n = 0      implicit line equation
    // X • n = P • n
    // x * nx + y * ny = P • n
    // x = (P • n - y * ny) / nx
    
    Vec norm {p1.y - p2.y, p2.x - p1.x};
    
    constexpr float Epsillion = 0.00001;
    
    if(fabs(norm.x) < Epsillion) {
        return 0.5f * (p1.x + p2.x);
    }
    
    return (dot(p1, norm) - py * norm.y) / norm.x;
}

// Finds 2 segmets that the horizontal line passing through the test point intersects O(log(n))
// Then if found, finds the x coordinate of those points to check if the test point x coordinate is between those O(1)
// Assumes that the polygon is convex and points are in counterclockwise order
template <size_t N>
std::vector<bool> isInside_O_LOG_N(const Polygon<N>& polygon, const std::vector<Vec>& points) {
    
    static_assert(N > 2);
    
    // Find top and bottom points
    auto tpi = polygon.points.begin();
    auto bpi = polygon.points.begin();
    
    for(auto i = polygon.points.begin() + 1; i != polygon.points.end(); ++i) {
        if(tpi->y < i->y) {
            tpi = i;
        } else if(bpi->y > i->y) {
            bpi = i;
        }
    }
    
    // Find 2 segments of the polygon that horizontal line (point.y) intersects
    auto tpCPI = polygon.cyclicPointIterator(tpi - polygon.points.begin());
    auto bpCPI = polygon.cyclicPointIterator(bpi - polygon.points.begin());
    
    std::vector<bool> result;
    result.reserve(points.size());
    for(const auto& point: points) {
        auto s1 = findIntersectionSegment(polygon, tpCPI, bpCPI, point.y, WindingOrder::Counterclockwise);
        if(!std::get<2>(s1)) {
            result.emplace_back(false);
            continue;
        }
        
        auto s2 = findIntersectionSegment(polygon, tpCPI, bpCPI, point.y, WindingOrder::Clockwise);
        assert(std::get<2>(s2));
        
        // Compute intersection points x
        auto& [s1p1, s1p2, dummy1] = s1;
        auto x1 = computePointXOnLine(s1p1, s1p2, point.y);
        
        auto& [s2p1, s2p2, dummy2] = s2;
        auto x2 = computePointXOnLine(s2p1, s2p2, point.y);
        
        auto range = std::minmax(x1, x2);
        
        result.emplace_back(point.x >= range.first && point.x <= range.second);
    }
    return result;
}

/*bool testPolygon() {
    Polygon<8> polygon;
    polygon.points = {{
        {-2.73843, 2.71944}, {-2.4654, 1.9405},
        {-1.45359, 1.22581}, {-0.10451, 0.79218},
        {1.91912, 1.73171}, {1.77457, 3.70716},
        {-0.12057, 4.79927}, {-2.49752, 4.21306},
    }};
    
    Vec point {-1.36541, 5.53606};
    
    return isInside_O_LOG_N(polygon, point);
//    return isInside_O_N(polygon, point);
    
}*/

void testTriangle() {
    Polygon<3> polygon;
    polygon.points = {{
        {-2.73889, 4.93378}, {0.50187, 1.62295},
        {1.37775, 6.01988}
    }};
    
    auto isInside = isInside_O_LOG_N<3>;
//    auto isInside = isInside_O_N<3>;
     
    assert( isInside(polygon, {{0.92388, 5.51323}}).front() );
    assert( isInside(polygon, {{-2.15714, 4.79357}}).front() );
    assert( isInside(polygon, {{0.36165, 2.16233}}).front() );
    assert( isInside(polygon, {{-0.20058, 4.1189}}).front() );

    assert( !isInside(polygon, {{2.85796, 4.05143}}).front() );
    assert( !isInside(polygon, {{3.01538, 8.25692}}).front() );
    assert( !isInside(polygon, {{-1.16762, 7.76215}}).front() );
    assert( !isInside(polygon, {{-4.78838, 7.08748}}).front() );
    assert( !isInside(polygon, {{-4.60847, 3.2643}}).front() );
    assert( !isInside(polygon, {{0.02431, 0.3407}}).front() );
    assert( !isInside(polygon, {{2.9704, 0.52062}}).front() );
    assert( !isInside(polygon, {{1.1038, 3.15186}}).front() );
    assert( !isInside(polygon, {{-0.67285, 5.76061}}).front() );
    assert( !isInside(polygon, {{-1.88727, 3.42173}}).front() );
    
}

template <size_t N>
void profile(Polygon<N> polygon) {
    
    auto isInside = isInside_O_LOG_N<3>;
    
    std::random_device rd;
    std::mt19937 gen(rd());
    std::uniform_real_distribution<float> dist{-5.0f, 5.0f};
    
    constexpr size_t PointCount = 100000;
    std::vector<Vec> points;
    points.reserve(PointCount);
    for(int i = 0; i < PointCount; ++i) {
        points.emplace_back(Vec {dist(gen), dist(gen)});
    }
    
    std::chrono::steady_clock::time_point begin = std::chrono::steady_clock::now();
    isInside(polygon, points);
    std::chrono::steady_clock::time_point end = std::chrono::steady_clock::now();
    
    std::cout << "Time difference = " << std::chrono::duration_cast<std::chrono::microseconds>(end - begin).count() << "[µs]" << std::endl;
}

int main(int argc, const char * argv[]) {
    
//    testTriangle();
    
    
    
    return 0;
}
